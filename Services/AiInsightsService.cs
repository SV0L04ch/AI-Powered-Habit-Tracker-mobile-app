using HabitApi.Models.DTO;
using HabitApi.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HabitApi.Services;

/// <summary>
/// Сервис для генерации ИИ-советов, аналитики и текстовых сводок.
/// Взаимодействует с LLM-провайдером (GroqCloud) через HTTP API.
/// </summary>
public sealed class AiInsightsService : IAiInsightsService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _model;
    private readonly string _endpoint;
    private readonly ILogger<AiInsightsService> _logger;

    private class ChatMessage
    {
        [JsonPropertyName("role")]
        public string Role { get; set; } = string.Empty;
        [JsonPropertyName("content")]
        public string Content { get; set; } = string.Empty;
    }

    private class ChatRequest
    {
        [JsonPropertyName("model")]
        public string Model { get; set; } = string.Empty;
        [JsonPropertyName("messages")]
        public List<ChatMessage> Messages { get; set; } = new();
    }

    private class ChatChoice
    {
        [JsonPropertyName("message")]
        public ChatMessage? Message { get; set; }
    }

    private class ChatResponse
    {
        [JsonPropertyName("choices")]
        public List<ChatChoice> Choices { get; set; } = new();
    }

    /// <summary>
    /// Инициализирует сервис ИИ с HTTP-клиентом, конфигурацией и логированием.
    /// </summary>
    /// <param name="httpClient">HTTP-клиент для запросов к LLM API.</param>
    /// <param name="configuration">Конфигурация приложения.</param>
    /// <param name="logger">Логгер для диагностики.</param>
    public AiInsightsService(HttpClient httpClient, IConfiguration configuration, ILogger<AiInsightsService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
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

    /// <inheritdoc />
    public async Task<string> BuildHabitSupportMessageAsync(string habitName, string scenario, CancellationToken cancellationToken)
    {
        var systemPrompt = "You are a supportive habit coach. Give a short, warm, and actionable message (1-3 sentences).";
        var userPrompt = scenario.Trim().ToLowerInvariant() switch
        {
            "lazy" => $"I'm feeling lazy about '{habitName}'. Give me a nudge.",
            "relapse" => $"I slipped on '{habitName}'. Help me recover.",
            "skip" => $"I skipped '{habitName}'. What now?",
            _ => $"Motivate me for '{habitName}'."
        };
        return await SendChatRequestAsync(systemPrompt, userPrompt, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<string> BuildDailyInsightAsync(DailySummaryDto summary, CancellationToken cancellationToken)
    {
        var systemPrompt = "You are a wellness analyst. Give a brief insight and suggestion based on the user's day.";
        var userPrompt = $"Today I completed {summary.HabitsCompleted}, partially {summary.HabitsPartiallyCompleted}, skipped {summary.HabitsSkipped}. Weather: {summary.Weather?.Condition}. Give me a 2-sentence insight.";
        return await SendChatRequestAsync(systemPrompt, userPrompt, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<string> BuildCitySummaryAsync(string city, List<CityHabitStatDto> stats, CancellationToken cancellationToken)
    {
        var systemPrompt = "You are a cheerful city reporter. Summarize top habits in a fun 1-sentence way.";
        var top = stats.Take(3).Select(s => $"{s.HabitName} ({s.Percentage:F0}%)");
        var userPrompt = $"Top habits in {city}: {string.Join(", ", top)}.";
        return await SendChatRequestAsync(systemPrompt, userPrompt, cancellationToken);
    }

    /// <summary>
    /// Отправляет запрос к LLM API и возвращает текст ответа.
    /// При ошибке сети или недоступности сервиса возвращает fallback-сообщение.
    /// </summary>
    private async Task<string> SendChatRequestAsync(string systemPrompt, string userPrompt, CancellationToken cancellationToken)
    {
        var request = new ChatRequest
        {
            Model = _model,
            Messages = new List<ChatMessage>
            {
                new() { Role = "system", Content = systemPrompt },
                new() { Role = "user", Content = userPrompt }
            }
        };

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, _endpoint)
        {
            Content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json")
        };
        httpRequest.Headers.Add("Authorization", $"Bearer {_apiKey}");

        try
        {
            var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
            response.EnsureSuccessStatusCode();

            var chatResponse = await response.Content.ReadFromJsonAsync<ChatResponse>(cancellationToken: cancellationToken);
            return chatResponse?.Choices?.FirstOrDefault()?.Message?.Content ?? "Stay consistent with your habits!";
        }
        catch (HttpRequestException ex)
        {
            _logger.LogWarning(ex, "AI request failed, returning fallback message");
            return "Keep going! Every small step counts.";
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogWarning(ex, "AI request timed out");
            return "Take a deep breath and try again later.";
        }
    }
}
