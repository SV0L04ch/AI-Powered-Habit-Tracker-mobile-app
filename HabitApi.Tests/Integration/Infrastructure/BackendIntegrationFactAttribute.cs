namespace HabitApi.Tests.Integration.Infrastructure;

internal sealed class BackendIntegrationFactAttribute : FactAttribute
{
    public BackendIntegrationFactAttribute()
    {
        if (!IntegrationTestSettings.BackendTestsEnabled)
        {
            Skip = "Set RUN_BACKEND_INTEGRATION_TESTS=1 to run Docker/Testcontainers backend tests.";
        }
    }
}
