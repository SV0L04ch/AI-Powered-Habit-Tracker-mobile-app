using System.Diagnostics;
using HabitApi.Models.DTO;
using HabitApi.Services;
using HabitApi.Tests.Integration.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit.Abstractions;

namespace HabitApi.Tests.Integration.AI;

[Collection(LlmIntegrationCollection.Name)]
[Trait("Category", "Integration")]
[Trait("Dependency", "Ollama")]
public sealed class AiInsightsServiceOllamaTests
{
    private readonly OllamaContainerFixture _ollama;
    private readonly ITestOutputHelper _output;

    public AiInsightsServiceOllamaTests(OllamaContainerFixture ollama, ITestOutputHelper output)
    {
        _ollama = ollama;
        _output = output;
    }

    [LlmIntegrationFact]
    public async Task BuildHabitSupportMessageAsync_OllamaContainer_ReturnsAnswerWithinResponseBudget()
    {
        using var env = new EnvironmentVariableScope(
            ("AI_BASE_URL", _ollama.ChatCompletionsEndpoint),
            ("AI_MODEL", _ollama.Model));

        using var httpClient = new HttpClient
        {
            Timeout = IntegrationTestSettings.ResponseTimeout + TimeSpan.FromSeconds(30)
        };

        var service = CreateService(httpClient);

        var stopwatch = Stopwatch.StartNew();
        var message = await service.BuildHabitSupportMessageAsync(
            "morning exercise",
            "lazy",
            CancellationToken.None);
        stopwatch.Stop();

        _output.WriteLine($"Ollama model: {_ollama.Model}");
        _output.WriteLine($"Endpoint: {_ollama.ChatCompletionsEndpoint}");
        _output.WriteLine($"Elapsed: {stopwatch.Elapsed}");
        _output.WriteLine($"Message: {message}");

        Assert.False(string.IsNullOrWhiteSpace(message));
        Assert.True(
            stopwatch.Elapsed <= IntegrationTestSettings.ResponseTimeout,
            $"LLM response took {stopwatch.Elapsed}, expected <= {IntegrationTestSettings.ResponseTimeout}.");
    }

    [LlmIntegrationFact]
    public async Task BuildDailyInsightAsync_OllamaContainer_ReturnsAnswer()
    {
        using var env = new EnvironmentVariableScope(
            ("AI_BASE_URL", _ollama.ChatCompletionsEndpoint),
            ("AI_MODEL", _ollama.Model));

        using var httpClient = new HttpClient
        {
            Timeout = IntegrationTestSettings.ResponseTimeout + TimeSpan.FromSeconds(30)
        };

        var service = CreateService(httpClient);
        var summary = new DailySummaryDto
        {
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            HabitsCompleted = 2,
            HabitsPartiallyCompleted = 1,
            HabitsSkipped = 1,
            Weather = new WeatherSnapshotDto
            {
                Condition = "Clouds",
                TemperatureCelsius = 16
            }
        };

        var message = await service.BuildDailyInsightAsync(summary, CancellationToken.None);

        _output.WriteLine($"Ollama model: {_ollama.Model}");
        _output.WriteLine($"Message: {message}");

        Assert.False(string.IsNullOrWhiteSpace(message));
    }

    private static AiInsightsService CreateService(HttpClient httpClient)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["AiApi:BaseUrl"] = Environment.GetEnvironmentVariable("AI_BASE_URL"),
                ["AiApi:Model"] = Environment.GetEnvironmentVariable("AI_MODEL"),
                ["AiApi:ApiKey"] = "ollama"
            })
            .Build();

        var logger = new Mock<ILogger<AiInsightsService>>().Object;
        return new AiInsightsService(httpClient, configuration, logger);
    }
}
