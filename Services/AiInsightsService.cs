using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using HabitApi.Models.DTO;
using HabitApi.Services.Interfaces;

namespace HabitApi.Services;

public sealed class AiInsightsService : IAiInsightsService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _model;
    private readonly string _endpoint;

    private static readonly JsonSerializerOptions PromptJsonOptions = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter() }
    };

    private sealed class ChatMessage
    {
        [JsonPropertyName("role")]
        public string Role { get; set; } = string.Empty;

        [JsonPropertyName("content")]
        public string Content { get; set; } = string.Empty;
    }

    private sealed class ChatRequest
    {
        [JsonPropertyName("model")]
        public string Model { get; set; } = string.Empty;

        [JsonPropertyName("messages")]
        public List<ChatMessage> Messages { get; set; } = new();

        [JsonPropertyName("temperature")]
        public double Temperature { get; set; } = 0.55;

        [JsonPropertyName("max_tokens")]
        public int MaxTokens { get; set; } = 360;
    }

    private sealed class ChatChoice
    {
        [JsonPropertyName("message")]
        public ChatMessage? Message { get; set; }
    }

    private sealed class ChatResponse
    {
        [JsonPropertyName("choices")]
        public List<ChatChoice> Choices { get; set; } = new();
    }

    public AiInsightsService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = Environment.GetEnvironmentVariable("AI_API_KEY")
                   ?? configuration["AiApi:ApiKey"]
                   ?? throw new InvalidOperationException("AI_API_KEY is not configured.");
        _model = Environment.GetEnvironmentVariable("AI_MODEL")
                 ?? configuration["AiApi:Model"]
                 ?? "llama-3.1-8b-instant";
        _endpoint = Environment.GetEnvironmentVariable("AI_BASE_URL")
                    ?? configuration["AiApi:BaseUrl"]
                    ?? "https://api.groq.com/openai/v1/chat/completions";
    }

    public Task<string> BuildHabitSupportMessageAsync(string habitName, string scenario, CancellationToken cancellationToken)
    {
        var systemPrompt = """
            Ты — поддерживающий коуч по привычкам.
            Отвечай только на русском языке.
            Дай короткое, теплое и практичное сообщение в 1-3 предложениях.
            Не используй markdown и не упоминай, что ты ИИ.
            """;

        var normalizedScenario = scenario.Trim().ToLowerInvariant();
        var userPrompt = normalizedScenario switch
        {
            "lazy" => $"Пользователь ленится выполнять привычку \"{habitName}\". Нужен мягкий, но собранный толчок к действию.",
            "relapse" => $"Пользователь сорвался по привычке \"{habitName}\". Нужна короткая поддержка без осуждения и с шагом восстановления.",
            "skip" => $"Пользователь пропустил привычку \"{habitName}\". Нужен краткий совет, как вернуться в ритм.",
            _ => $"Нужна мотивация для привычки \"{habitName}\"."
        };

        return SendChatRequestAsync(systemPrompt, userPrompt, cancellationToken);
    }

    public Task<string> BuildDailyInsightAsync(DailySummaryDto summary, CancellationToken cancellationToken)
    {
        var systemPrompt = """
            Ты — аналитик привычек и доброжелательный наставник.
            Отвечай только на русском языке.
            Сформулируй краткую персональную сводку в 2-3 предложениях.
            Если погода отсутствует, не придумывай её.
            Заверши ответ мягкой мотивацией или следующим шагом.
            Не используй markdown и не упоминай, что ты ИИ.
            """;

        var userPrompt = $$"""
            Вот данные за день:
            {
              "date": "{{summary.Date}}",
              "habitsCompleted": {{summary.HabitsCompleted}},
              "habitsPartiallyCompleted": {{summary.HabitsPartiallyCompleted}},
              "habitsSkipped": {{summary.HabitsSkipped}},
              "weather": {{JsonSerializer.Serialize(summary.Weather, PromptJsonOptions)}}
            }

            Нужен короткий вывод по дню и одна поддерживающая рекомендация.
            """;

        return SendChatRequestAsync(systemPrompt, userPrompt, cancellationToken);
    }

    public Task<string> BuildHabitWeatherInsightAsync(HabitWeatherInsightResponseDto summary, CancellationToken cancellationToken)
    {
        var systemPrompt = """
            Ты — внимательный аналитик привычек и мотивирующий коуч для мобильного приложения.
            Отвечай только на русском языке.
            Тебе дают данные по одной привычке, текущему дню, опционально предыдущему дню и погоде OpenWeather.
            Правила ответа:
            1. Напиши 3-4 коротких предложения одним абзацем.
            2. Начни с факта за текущий день: погода и результат привычки.
            3. Если есть предыдущий день, сравни погоду и результат без длинных вычислений.
            4. Связывай погоду с поведением как осторожное наблюдение: "похоже", "может быть", "стоит проверить".
            5. Не утверждай, что погода точно стала причиной поведения.
            6. Для полезной привычки лучший результат — Completed или рост PartialValue.
            7. Для вредной привычки лучший результат — меньшее число RelapseCount; называй это обычным языком, например "срывов меньше" или "сигарет меньше", если это ясно из названия привычки.
            8. Если записи или погоды за какой-то день нет, не придумывай данные и честно скажи, на чем основан вывод.
            9. Заверши короткой поддержкой и одним практическим следующим шагом на завтра или сегодня.
            Не используй markdown и не упоминай, что ты ИИ.
            """;

        var promptPayload = new
        {
            habit = new
            {
                id = summary.HabitId,
                name = summary.HabitName,
                type = summary.IsPositive ? "positive_habit" : "negative_habit",
                interpretation = summary.IsPositive
                    ? "Пользователь старается выполнять эту привычку."
                    : "Пользователь старается уменьшить или исключить это поведение."
            },
            targetDate = summary.Date,
            currentDay = summary.CurrentDay,
            previousDay = summary.PreviousDay,
            outputGoal = "Сводка для пользователя после нажатия кнопки в приложении: связать погоду с результатом привычки, сравнить с предыдущим днем при наличии данных и дать мотивацию."
        };

        var userPrompt = $"""
            Подготовь персональную сводку по этим данным. Используй только факты из JSON:
            {JsonSerializer.Serialize(promptPayload, PromptJsonOptions)}
            """;

        return SendChatRequestAsync(systemPrompt, userPrompt, cancellationToken);
    }

    public Task<string> BuildCitySummaryAsync(string city, List<CityHabitStatDto> stats, CancellationToken cancellationToken)
    {
        var systemPrompt = """
            Ты — городской обозреватель привычек.
            Отвечай только на русском языке.
            Дай одну короткую фразу без markdown.
            """;

        var top = stats.Take(3).Select(s => $"{s.HabitName} ({s.Percentage:F0}%)");
        var userPrompt = $"Сделай короткую сводку по городу {city}. Топ привычек: {string.Join(", ", top)}.";

        return SendChatRequestAsync(systemPrompt, userPrompt, cancellationToken);
    }

    private async Task<string> SendChatRequestAsync(string systemPrompt, string userPrompt, CancellationToken cancellationToken)
    {
        var request = new ChatRequest
        {
            Model = _model,
            Messages =
            [
                new ChatMessage { Role = "system", Content = systemPrompt },
                new ChatMessage { Role = "user", Content = userPrompt }
            ]
        };

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, _endpoint)
        {
            Content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json")
        };
        httpRequest.Headers.Add("Authorization", $"Bearer {_apiKey}");

        var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
        response.EnsureSuccessStatusCode();

        var chatResponse = await response.Content.ReadFromJsonAsync<ChatResponse>(cancellationToken: cancellationToken);
        return chatResponse?.Choices?.FirstOrDefault()?.Message?.Content?.Trim()
               ?? "Продолжайте в том же духе — у вас уже есть хороший задел.";
    }
}
