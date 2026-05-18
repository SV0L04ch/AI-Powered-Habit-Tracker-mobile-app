namespace HabitApi.Tests.Integration.Infrastructure;

internal sealed class EnvironmentVariableScope : IDisposable
{
    private readonly Dictionary<string, string?> _previousValues = new();

    public EnvironmentVariableScope(params (string Name, string? Value)[] variables)
    {
        foreach (var (name, value) in variables)
        {
            _previousValues[name] = Environment.GetEnvironmentVariable(name);
            Environment.SetEnvironmentVariable(name, value);
        }
    }

    public void Dispose()
    {
        foreach (var (name, value) in _previousValues)
        {
            Environment.SetEnvironmentVariable(name, value);
        }
    }
}
