namespace HabitApi.Tests.Integration.Infrastructure;

internal static class IntegrationTestSettings
{
    public static bool LlmTestsEnabled => IsEnabled("RUN_LLM_INTEGRATION_TESTS");

    public static bool BackendTestsEnabled => IsEnabled("RUN_BACKEND_INTEGRATION_TESTS");

    public static string PostgresImage =>
        GetString("BACKEND_TEST_POSTGRES_IMAGE", "postgres:15-alpine");

    public static TimeSpan PostgresStartupTimeout =>
        TimeSpan.FromSeconds(GetInt("BACKEND_TEST_POSTGRES_STARTUP_TIMEOUT_SECONDS", 120));

    public static string OllamaImage =>
        GetString("LLM_TEST_OLLAMA_IMAGE", "ollama/ollama:latest");

    public static string LlmModel =>
        GetString("LLM_TEST_MODEL", "gemma3:4b");

    public static bool PullModel =>
        !IsDisabled("LLM_TEST_PULL_MODEL");

    public static TimeSpan StartupTimeout =>
        TimeSpan.FromSeconds(GetInt("LLM_TEST_STARTUP_TIMEOUT_SECONDS", 300));

    public static TimeSpan ModelPullTimeout =>
        TimeSpan.FromSeconds(GetInt("LLM_TEST_MODEL_PULL_TIMEOUT_SECONDS", 1200));

    public static TimeSpan ResponseTimeout =>
        TimeSpan.FromSeconds(GetInt("LLM_TEST_RESPONSE_TIMEOUT_SECONDS", 180));

    private static string GetString(string name, string defaultValue)
    {
        var value = Environment.GetEnvironmentVariable(name);
        return string.IsNullOrWhiteSpace(value) ? defaultValue : value;
    }

    private static int GetInt(string name, int defaultValue)
    {
        var value = Environment.GetEnvironmentVariable(name);
        return int.TryParse(value, out var parsed) && parsed > 0 ? parsed : defaultValue;
    }

    private static bool IsEnabled(string name)
    {
        var value = Environment.GetEnvironmentVariable(name);
        return string.Equals(value, "1", StringComparison.OrdinalIgnoreCase)
            || string.Equals(value, "true", StringComparison.OrdinalIgnoreCase)
            || string.Equals(value, "yes", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsDisabled(string name)
    {
        var value = Environment.GetEnvironmentVariable(name);
        return string.Equals(value, "0", StringComparison.OrdinalIgnoreCase)
            || string.Equals(value, "false", StringComparison.OrdinalIgnoreCase)
            || string.Equals(value, "no", StringComparison.OrdinalIgnoreCase);
    }
}
