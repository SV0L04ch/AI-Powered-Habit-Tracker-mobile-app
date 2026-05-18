using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using HabitApi.Data;
using Microsoft.EntityFrameworkCore;

namespace HabitApi.Tests.Integration.Infrastructure;

public sealed class PostgresContainerFixture : IAsyncLifetime
{
    private const int PostgresPort = 5432;
    private const string Database = "habit_tracker_tests";
    private const string Username = "habit_user";
    private const string Password = "habit_password";

    private readonly IContainer _container;

    public PostgresContainerFixture()
    {
        _container = new ContainerBuilder(IntegrationTestSettings.PostgresImage)
            .WithEnvironment("POSTGRES_DB", Database)
            .WithEnvironment("POSTGRES_USER", Username)
            .WithEnvironment("POSTGRES_PASSWORD", Password)
            .WithPortBinding(PostgresPort, true)
            .WithWaitStrategy(Wait.ForUnixContainer()
                .UntilCommandIsCompleted($"pg_isready -U {Username} -d {Database}", strategy => strategy
                    .WithInterval(TimeSpan.FromSeconds(1))
                    .WithTimeout(IntegrationTestSettings.PostgresStartupTimeout)))
            .Build();
    }

    public string ConnectionString =>
        $"Host={_container.Hostname};Port={_container.GetMappedPublicPort(PostgresPort)};Database={Database};Username={Username};Password={Password}";

    public Task InitializeAsync() => _container.StartAsync();

    public async Task ResetDatabaseAsync()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(ConnectionString)
            .Options;

        await using var dbContext = new AppDbContext(options);
        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await _container.DisposeAsync();
    }
}
