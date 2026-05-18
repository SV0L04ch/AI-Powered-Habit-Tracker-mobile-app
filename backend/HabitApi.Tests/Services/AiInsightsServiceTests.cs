using Xunit;
using Moq;
using RichardSzalay.MockHttp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using HabitApi.Services;
using HabitApi.Models.DTO;
using System.Net;
using System.Text;
using System.Text.Json;

namespace HabitApi.Tests.Services;

/// <summary>
/// Модульные тесты для <see cref="AiInsightsService"/>.
/// Проверяет генерацию мотивационных сообщений, аналитики и городских сводок через локальную Ollama.
/// </summary>
public class AiInsightsServiceTests
{
    /// <summary>
    /// Создаёт экземпляр <see cref="AiInsightsService"/> с замоканным HttpClient, конфигурацией и логгером.
    /// </summary>
    private static AiInsightsService CreateService(HttpClient httpClient, string model = "test-model", string endpoint = "http://localhost:11434/v1/chat/completions")
    {
        var configBuilder = new ConfigurationBuilder();
        configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
        {
            { "AiApi:Model", model },
            { "AiApi:BaseUrl", endpoint },
            { "AiApi:ApiKey", "fake-key" }
        });
        var configuration = configBuilder.Build();

        var logger = new Mock<ILogger<AiInsightsService>>().Object;
        return new AiInsightsService(httpClient, configuration, logger);
    }

    /// <summary>
    /// Проверяет, что при успешном ответе от Ollama возвращается осмысленное сообщение.
    /// </summary>
    [Fact]
    public async Task BuildHabitSupportMessageAsync_ValidRequest_ReturnsContent()
    {
        // Arrange
        var expectedReply = "Ты молодец, продолжай!";
        var ollamaResponse = new
        {
            choices = new[]
            {
                new { message = new { content = expectedReply } }
            }
        };

        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When("http://localhost:11434/v1/chat/completions")
                .Respond("application/json", JsonSerializer.Serialize(ollamaResponse));

        var service = CreateService(mockHttp.ToHttpClient());

        // Act
        var result = await service.BuildHabitSupportMessageAsync("Спорт", "lazy", CancellationToken.None);

        // Assert
        Assert.Equal(expectedReply, result);
    }

    /// <summary>
    /// Проверяет, что при сетевой ошибке возвращается fallback-сообщение, а не исключение.
    /// </summary>
    [Fact]
    public async Task BuildHabitSupportMessageAsync_NetworkError_ReturnsFallback()
    {
        // Arrange
        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When("http://localhost:11434/v1/chat/completions")
                .Respond(HttpStatusCode.InternalServerError);

        var service = CreateService(mockHttp.ToHttpClient());

        // Act
        var result = await service.BuildHabitSupportMessageAsync("Чтение", "relapse", CancellationToken.None);

        // Assert
        Assert.Equal("Продолжай в том же духе! Каждый шаг важен.", result); // текущий fallback
    }

    /// <summary>
    /// Проверяет, что при тайм-ауте также возвращается fallback.
    /// </summary>
    [Fact]
    public async Task BuildHabitSupportMessageAsync_Timeout_ReturnsFallback()
    {
        // Arrange
        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When("http://localhost:11434/v1/chat/completions")
                .Respond(async _ =>
                {
                    await Task.Delay(TimeSpan.FromSeconds(10)); // дольше тайм-аута HttpClient
                    return new HttpResponseMessage(HttpStatusCode.OK);
                });

        var httpClient = mockHttp.ToHttpClient();
        httpClient.Timeout = TimeSpan.FromMilliseconds(100); // очень короткий тайм-аут

        var service = CreateService(httpClient);

        // Act
        var result = await service.BuildHabitSupportMessageAsync("Сон", "skip", CancellationToken.None);

        // Assert
        Assert.Equal("Продолжай в том же духе! Каждый шаг важен.", result);
    }

    /// <summary>
    /// Проверяет, что ежедневная аналитика формируется корректно.
    /// </summary>
    [Fact]
    public async Task BuildDailyInsightAsync_ReturnsContent()
    {
        // Arrange
        var summary = new DailySummaryDto
        {
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            HabitsCompleted = 3,
            HabitsPartiallyCompleted = 1,
            HabitsSkipped = 0,
            Weather = null
        };

        var ollamaResponse = new
        {
            choices = new[]
            {
                new { message = new { content = "Отличный день! Так держать." } }
            }
        };

        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When("http://localhost:11434/v1/chat/completions")
                .Respond("application/json", JsonSerializer.Serialize(ollamaResponse));

        var service = CreateService(mockHttp.ToHttpClient());

        // Act
        var result = await service.BuildDailyInsightAsync(summary, CancellationToken.None);

        // Assert
        Assert.Equal("Отличный день! Так держать.", result);
    }

    /// <summary>
    /// Проверяет, что городская сводка генерируется на основе переданной статистики.
    /// </summary>
    [Fact]
    public async Task BuildCitySummaryAsync_ReturnsContent()
    {
        // Arrange
        var city = "Moscow";
        var stats = new List<CityHabitStatDto>
        {
            new() { HabitName = "Бег", UserCount = 150, TotalUsers = 1000 },
            new() { HabitName = "Чтение", UserCount = 200, TotalUsers = 1000 }
        };

        var ollamaResponse = new
        {
            choices = new[]
            {
                new { message = new { content = "В Москве самые популярные привычки: бег и чтение!" } }
            }
        };

        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When("http://localhost:11434/v1/chat/completions")
                .Respond("application/json", JsonSerializer.Serialize(ollamaResponse));

        var service = CreateService(mockHttp.ToHttpClient());

        // Act
        var result = await service.BuildCitySummaryAsync(city, stats, CancellationToken.None);

        // Assert
        Assert.Equal("В Москве самые популярные привычки: бег и чтение!", result);
    }
}
