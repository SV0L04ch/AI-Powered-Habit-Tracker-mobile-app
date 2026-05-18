namespace HabitApi.Tests.Integration.Infrastructure;

[CollectionDefinition(Name, DisableParallelization = true)]
public sealed class BackendIntegrationCollection : ICollectionFixture<PostgresContainerFixture>
{
    public const string Name = "Backend integration tests";
}
