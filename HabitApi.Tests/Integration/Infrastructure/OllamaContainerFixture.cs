using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;

namespace HabitApi.Tests.Integration.Infrastructure;

public sealed class OllamaContainerFixture : IAsyncLifetime
{
    private const int OllamaPort = 11434;

    private readonly IContainer _container;

    public OllamaContainerFixture()
    {
        _container = new ContainerBuilder(IntegrationTestSettings.OllamaImage)
            .WithPortBinding(OllamaPort, true)
            .WithEnvironment("OLLAMA_HOST", "0.0.0.0")
            .WithEnvironment("OLLAMA_KEEP_ALIVE", "24h")
            .WithWaitStrategy(Wait.ForUnixContainer()
                .UntilHttpRequestIsSucceeded(request => request
                    .ForPort(OllamaPort)
                    .ForPath("/api/tags"), strategy => strategy
                    .WithInterval(TimeSpan.FromSeconds(1))
                    .WithTimeout(IntegrationTestSettings.StartupTimeout)))
            .Build();
    }

    public string Model => IntegrationTestSettings.LlmModel;

    public string ChatCompletionsEndpoint =>
        $"http://{_container.Hostname}:{_container.GetMappedPublicPort(OllamaPort)}/v1/chat/completions";

    public async Task InitializeAsync()
    {
        await _container.StartAsync();

        if (!IntegrationTestSettings.PullModel)
        {
            return;
        }

        using var cts = new CancellationTokenSource(IntegrationTestSettings.ModelPullTimeout);
        var result = await _container.ExecAsync(
            new[] { "ollama", "pull", IntegrationTestSettings.LlmModel },
            cts.Token);

        if (result.ExitCode != 0)
        {
            throw new InvalidOperationException(
                $"Failed to pull Ollama model '{IntegrationTestSettings.LlmModel}'. " +
                $"stdout: {result.Stdout}; stderr: {result.Stderr}");
        }
    }

    public async Task DisposeAsync()
    {
        await _container.DisposeAsync();
    }
}
