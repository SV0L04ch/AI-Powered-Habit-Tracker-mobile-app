namespace HabitApi.Tests.Integration.Infrastructure;

[CollectionDefinition(Name, DisableParallelization = true)]
public sealed class LlmIntegrationCollection : ICollectionFixture<OllamaContainerFixture>
{
    public const string Name = "LLM integration tests";
}
