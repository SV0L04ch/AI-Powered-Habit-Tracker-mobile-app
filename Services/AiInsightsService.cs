using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using HabitApi.Models.DTO;
using HabitApi.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace HabitApi.Services;

/// <summary>
/// Сервис для генерации ИИ-советов через локальную Ollama (OpenAI-совместимый API).
/// </summary>
public sealed class AiInsightsService : IAiInsightsService
{
    private readonly HttpClient _httpClient;
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
        [JsonPropertyName("stream")]
        public bool Stream { get; set; } = false;
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

    public AiInsightsService(HttpClient httpClient, IConfiguration configuration, ILogger<AiInsightsService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;

        _model = Environment.GetEnvironmentVariable("AI_MODEL")
                 ?? configuration["AiApi:Model"]
                 ?? "gemma4";

        var baseUrl = Environment.GetEnvironmentVariable("AI_BASE_URL")
                      ?? configuration["AiApi:BaseUrl"]
                      ?? "http://ollama:11434/v1";

        // Гарантируем, что endpoint заканчивается на /chat/completions (без дублирования)
        _endpoint = baseUrl.TrimEnd('/');
        if (!_endpoint.EndsWith("/chat/completions", StringComparison.OrdinalIgnoreCase))
        {
            _endpoint += "/chat/completions";
        }
    }

    /// <inheritdoc />
    public async Task<string> BuildHabitSupportMessageAsync(string habitName, string scenario, CancellationToken cancellationToken)
    {
        var systemPrompt = "You are a supportive habit coach. Reply in Russian. Give a short, warm, and actionable message (1-3 sentences).";
        var userPrompt = scenario.Trim().ToLowerInvariant() switch
        {
            "lazy" => $"Я ленюсь выполнять привычку '{habitName}'. Дай мне мотивации.",
            "relapse" => $"У меня случился срыв с привычкой '{habitName}'. Помоги мне вернуться в строй.",
            "skip" => $"Я пропустил выполнение привычки '{habitName}'. Что мне теперь делать?",
            _ => $"Мне нужна мотивация для привычки '{habitName}'."
        };
        return await SendChatRequestAsync(systemPrompt, userPrompt, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<string> BuildDailyInsightAsync(DailySummaryDto summary, CancellationToken cancellationToken)
    {
        var systemPrompt = "You are a wellness analyst. Reply in Russian. Give a brief insight and suggestion based on the user's day.";
        var userPrompt = $"Сегодня я выполнил {summary.HabitsCompleted} привычек, частично {summary.HabitsPartiallyCompleted}, пропустил {summary.HabitsSkipped}. Погода: {summary.Weather?.Condition}. Дай совет на 1-2 предложения.";
        return await SendChatRequestAsync(systemPrompt, userPrompt, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<string> BuildCitySummaryAsync(string city, List<CityHabitStatDto> stats, CancellationToken cancellationToken)
    {
        var systemPrompt = "You are a cheerful city reporter. Reply in Russian. Summarize top habits in a fun 1-sentence way.";
        var top = stats.Take(3).Select(s => $"{s.HabitName} ({s.Percentage:F0}%)");
        var userPrompt = $"Топ привычек в городе {city}: {string.Join(", ", top)}.";
        return await SendChatRequestAsync(systemPrompt, userPrompt, cancellationToken);
    }

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

        var jsonRequest = JsonSerializer.Serialize(request);
        _logger.LogDebug("Ollama request to {Endpoint}: {Request}", _endpoint, jsonRequest);

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, _endpoint)
        {
            Content = new StringContent(jsonRequest, Encoding.UTF8, "application/json")
        };

        try
        {
            var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
            response.EnsureSuccessStatusCode();

            var chatResponse = await response.Content.ReadFromJsonAsync<ChatResponse>(cancellationToken: cancellationToken);
            var message = chatResponse?.Choices?.FirstOrDefault()?.Message?.Content;
            if (!string.IsNullOrWhiteSpace(message))
            {
                _logger.LogDebug("Ollama response: {Message}", message);
                return message;
            }
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or JsonException)
        {
            _logger.LogWarning(ex, "Failed to get response from Ollama");
        }

        return "Продолжай в том же духе! Каждый шаг важен.";
    }
}
