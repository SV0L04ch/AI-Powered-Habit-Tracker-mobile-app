using HabitApi.Models.DTO;
using HabitApi.Services.Interfaces;

namespace HabitApi.Tests.Integration.Infrastructure;

internal sealed class TestAiInsightsService : IAiInsightsService
{
    public Task<string> BuildDailyInsightAsync(DailySummaryDto summary, CancellationToken cancellationToken)
    {
        return Task.FromResult($"Completed: {summary.HabitsCompleted}; skipped: {summary.HabitsSkipped}.");
    }

    public Task<string> BuildHabitSupportMessageAsync(string habitName, string scenario, CancellationToken cancellationToken)
    {
        return Task.FromResult($"Keep going with {habitName}.");
    }

    public Task<string> BuildCitySummaryAsync(string city, List<CityHabitStatDto> stats, CancellationToken cancellationToken)
    {
        return Task.FromResult($"{city}: {stats.Count} habits.");
    }
}
