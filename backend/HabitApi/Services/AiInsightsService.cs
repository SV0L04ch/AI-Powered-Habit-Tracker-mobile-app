using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using HabitApi.Models.DTO;
using HabitApi.Services.Interfaces;

namespace HabitApi.Services;

/// <summary>
/// Generates AI insights through either an OpenAI-compatible chat endpoint
/// or native Ollama /api/chat.
/// </summary>
public sealed class AiInsightsService : IAiInsightsService
{
    private const string AiFallbackReason = "AI service is temporarily unavailable.";

    private readonly HttpClient _httpClient;
    private readonly string _model;
    private readonly string _baseUrl;
    private readonly string? _apiKey;
    private readonly ILogger<AiInsightsService> _logger;

    private sealed class ChatMessage
    {
        [JsonPropertyName("role")]
        public string Role { get; set; } = string.Empty;

        [JsonPropertyName("content")]
        public string Content { get; set; } = string.Empty;
    }

    private sealed class OpenAiChatRequest
    {
        [JsonPropertyName("model")]
        public string Model { get; set; } = string.Empty;

        [JsonPropertyName("messages")]
        public List<ChatMessage> Messages { get; set; } = new();

        [JsonPropertyName("stream")]
        public bool Stream { get; set; } = false;
    }

    private sealed class OpenAiChatChoice
    {
        [JsonPropertyName("message")]
        public ChatMessage? Message { get; set; }
    }

    private sealed class OpenAiChatResponse
    {
        [JsonPropertyName("choices")]
        public List<OpenAiChatChoice> Choices { get; set; } = new();
    }

    private sealed class OllamaChatRequest
    {
        [JsonPropertyName("model")]
        public string Model { get; set; } = string.Empty;

        [JsonPropertyName("messages")]
        public List<ChatMessage> Messages { get; set; } = new();

        [JsonPropertyName("stream")]
        public bool Stream { get; set; } = false;
    }

    private sealed class OllamaChatResponse
    {
        [JsonPropertyName("message")]
        public ChatMessage? Message { get; set; }

        [JsonPropertyName("response")]
        public string? Response { get; set; }
    }

    public AiInsightsService(HttpClient httpClient, IConfiguration configuration, ILogger<AiInsightsService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;

        _model = Environment.GetEnvironmentVariable("AI_MODEL")
                 ?? configuration["AiApi:Model"]
                 ?? "lakomoor/vikhr-llama-3.2-1b-instruct:q5_k_m";

        _baseUrl = (Environment.GetEnvironmentVariable("AI_BASE_URL")
                   ?? configuration["AiApi:BaseUrl"]
                   ?? "http://ollama:11434").TrimEnd('/');

        _apiKey = Environment.GetEnvironmentVariable("AI_API_KEY")
                  ?? configuration["AiApi:ApiKey"];
    }

    public async Task<AiInsightResultDto> BuildHabitSupportMessageAsync(
        string habitName,
        string scenario,
        CancellationToken cancellationToken)
    {
        var normalizedScenario = NormalizeScenario(scenario);
        var systemPrompt =
            "Ты доброжелательный habit coach. Отвечай на русском. " +
            "Дай короткий, конкретный совет на 1-3 предложения без морализаторства.";

        var userPrompt = normalizedScenario switch
        {
            "relapse" =>
                $"Пользователь сорвался в привычке \"{habitName}\". Помоги спокойно вернуться к плану.",
            "skip" =>
                $"Пользователь пропустил привычку \"{habitName}\". Дай мягкий план восстановления на сегодня.",
            _ =>
                $"Пользователь хочет выполнить привычку \"{habitName}\". Сгенерируй персональный мотивирующий совет."
        };

        return await SendChatRequestAsync(systemPrompt, userPrompt, () => BuildHabitFallback(habitName, normalizedScenario), cancellationToken);
    }

    public async Task<AiInsightResultDto> BuildDailyInsightAsync(DailySummaryDto summary, CancellationToken cancellationToken)
    {
        var total = summary.HabitsCompleted + summary.HabitsPartiallyCompleted + summary.HabitsSkipped;
        var percent = total > 0 ? Math.Round((double)summary.HabitsCompleted / total * 100) : 0;
        var weather = summary.Weather is null
            ? "погода недоступна"
            : $"{summary.Weather.Condition}, {summary.Weather.TemperatureCelsius}C";

        var systemPrompt =
            "Ты wellness analyst в приложении трекера привычек. Отвечай на русском. " +
            "Сделай отзыв по дню: если прогресс хороший - поддержи, если слабый - мягко предложи следующий шаг. " +
            "Не будь жестким и не придумывай факты.";

        var userPrompt =
            $"Дата: {summary.Date}. Выполнено: {summary.HabitsCompleted}. " +
            $"Частично: {summary.HabitsPartiallyCompleted}. Пропущено: {summary.HabitsSkipped}. " +
            $"Процент выполнения: {percent}%. Погода: {weather}. " +
            "Напиши 1-2 предложения для daily summary.";

        return await SendChatRequestAsync(systemPrompt, userPrompt, () => BuildDailyFallback(summary), cancellationToken);
    }

    public async Task<AiInsightResultDto> BuildCitySummaryAsync(string city, List<CityHabitStatDto> stats, CancellationToken cancellationToken)
    {
        var top = stats.Take(3).Select(s => $"{s.HabitName} ({s.Percentage:F0}%)");
        var systemPrompt =
            "Ты краткий городской репортер wellness-приложения. Отвечай на русском одним теплым предложением.";
        var userPrompt = $"Город: {city}. Топ привычек: {string.Join(", ", top)}.";

        return await SendChatRequestAsync(systemPrompt, userPrompt, () => BuildCityFallback(city, stats), cancellationToken);
    }

    private async Task<AiInsightResultDto> SendChatRequestAsync(
        string systemPrompt,
        string userPrompt,
        Func<string> fallbackFactory,
        CancellationToken cancellationToken)
    {
        var messages = new List<ChatMessage>
        {
            new() { Role = "system", Content = systemPrompt },
            new() { Role = "user", Content = userPrompt }
        };

        try
        {
            // Если в базовом URL есть "ollama" – используем нативный API
            if (_baseUrl.Contains("ollama", StringComparison.OrdinalIgnoreCase))
            {
                return CreateSuccess(await SendOllamaRequestAsync(messages, cancellationToken));
            }
            // Иначе – стандартное определение
            if (IsOpenAiCompatibleBaseUrl(_baseUrl))
            {
                return CreateSuccess(await SendOpenAiCompatibleRequestAsync(messages, cancellationToken));
            }
            return CreateSuccess(await SendOllamaRequestAsync(messages, cancellationToken));
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound && IsOpenAiCompatibleBaseUrl(_baseUrl))
        {
            _logger.LogWarning(ex, "OpenAI-compatible AI endpoint returned 404, retrying native Ollama endpoint");
            try
            {
                return CreateSuccess(await SendOllamaRequestAsync(messages, cancellationToken));
            }
            catch (Exception ollamaEx) when (ollamaEx is HttpRequestException or TaskCanceledException or JsonException)
            {
                _logger.LogWarning(ollamaEx, "Native Ollama fallback failed");
            }
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or JsonException)
        {
            _logger.LogWarning(ex, "Failed to get AI response");
        }

        return CreateFallback(fallbackFactory());
    }

    private static AiInsightResultDto CreateSuccess(string message)
    {
        return new AiInsightResultDto
        {
            Message = message,
            IsFallback = false
        };
    }

    private static AiInsightResultDto CreateFallback(string message)
    {
        return new AiInsightResultDto
        {
            Message = message,
            IsFallback = true,
            FallbackReason = AiFallbackReason
        };
    }

    private async Task<string> SendOpenAiCompatibleRequestAsync(
        List<ChatMessage> messages,
        CancellationToken cancellationToken)
    {
        var endpoint = BuildOpenAiChatEndpoint();
        var request = new OpenAiChatRequest
        {
            Model = _model,
            Messages = messages
        };

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, endpoint)
        {
            Content = JsonContent.Create(request)
        };

        if (!string.IsNullOrWhiteSpace(_apiKey))
        {
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
        }

        var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
        response.EnsureSuccessStatusCode();

        var chatResponse = await response.Content.ReadFromJsonAsync<OpenAiChatResponse>(cancellationToken: cancellationToken);
        var message = chatResponse?.Choices?.FirstOrDefault()?.Message?.Content;
        if (string.IsNullOrWhiteSpace(message))
            throw new JsonException("AI response does not contain a message.");

        return message.Trim();
    }

    private async Task<string> SendOllamaRequestAsync(
        List<ChatMessage> messages,
        CancellationToken cancellationToken)
    {
        var endpoint = BuildOllamaChatEndpoint();
        var request = new OllamaChatRequest
        {
            Model = _model,
            Messages = messages
        };

        var response = await _httpClient.PostAsJsonAsync(endpoint, request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var chatResponse = await response.Content.ReadFromJsonAsync<OllamaChatResponse>(cancellationToken: cancellationToken);
        var message = chatResponse?.Message?.Content ?? chatResponse?.Response;
        if (string.IsNullOrWhiteSpace(message))
            throw new JsonException("Ollama response does not contain a message.");

        return message.Trim();
    }

    private string BuildOpenAiChatEndpoint()
    {
        if (_baseUrl.EndsWith("/chat/completions", StringComparison.OrdinalIgnoreCase))
            return _baseUrl;

        return $"{_baseUrl.TrimEnd('/')}/chat/completions";
    }

    private string BuildOllamaChatEndpoint()
    {
        var baseUrl = _baseUrl
            .Replace("/v1/chat/completions", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("/v1", string.Empty, StringComparison.OrdinalIgnoreCase)
            .TrimEnd('/');

        return $"{baseUrl}/api/chat";
    }

    private static bool IsOpenAiCompatibleBaseUrl(string baseUrl)
    {
        return baseUrl.Contains("/v1", StringComparison.OrdinalIgnoreCase)
               || !baseUrl.Contains("ollama", StringComparison.OrdinalIgnoreCase);
    }

    private static string NormalizeScenario(string scenario)
    {
        var normalized = scenario.Trim().ToLowerInvariant();
        return normalized is "relapse" or "skip" or "lazy" ? normalized : "lazy";
    }

    private static string BuildHabitFallback(string habitName, string scenario)
    {
        return scenario switch
        {
            "relapse" =>
                $"Срыв в \"{habitName}\" не обнуляет прогресс. Зафиксируй, что его спровоцировало, и выбери один маленький шаг для возвращения сегодня.",
            "skip" =>
                $"Если \"{habitName}\" сегодня пропущена, не дави на себя. Запланируй самый короткий вариант выполнения на завтра и сохрани ритм.",
            _ =>
                $"Для \"{habitName}\" начни с версии на две минуты. Часто самый маленький вход в действие уже возвращает ощущение контроля."
        };
    }

    private static string BuildDailyFallback(DailySummaryDto summary)
    {
        var total = summary.HabitsCompleted + summary.HabitsPartiallyCompleted + summary.HabitsSkipped;
        if (total == 0)
            return "Сегодня еще нет отметок. Выбери одну привычку и сделай самый простой первый шаг.";

        if (summary.HabitsCompleted >= summary.HabitsSkipped)
            return "День выглядит устойчиво: выполненные привычки держат темп. Завтра попробуй закрепить его одной ранней отметкой.";

        return "Сегодня было больше пропусков, чем хотелось бы, но это не провал. Выбери одну ключевую привычку и начни с короткого выполнения.";
    }

    private static string BuildCityFallback(string city, List<CityHabitStatDto> stats)
    {
        var topHabit = stats.OrderByDescending(s => s.UserCount).FirstOrDefault()?.HabitName;
        return topHabit is null
            ? $"В городе {city} пока мало данных, но первая статистика появится после новых отметок."
            : $"В городе {city} сейчас выделяется привычка \"{topHabit}\" - хороший ориентир для общего ритма недели.";
    }
}
