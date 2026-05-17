namespace HabitApi.Tests.Integration.Infrastructure;

internal sealed class LlmIntegrationFactAttribute : FactAttribute
{
    public LlmIntegrationFactAttribute()
    {
        if (!IntegrationTestSettings.LlmTestsEnabled)
        {
            Skip = "Set RUN_LLM_INTEGRATION_TESTS=1 to run Docker/Testcontainers LLM tests.";
        }
    }
}
