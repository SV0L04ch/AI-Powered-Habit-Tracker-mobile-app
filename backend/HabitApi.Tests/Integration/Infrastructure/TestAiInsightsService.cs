using HabitApi.Models.DTO;
using HabitApi.Services.Interfaces;

namespace HabitApi.Tests.Integration.Infrastructure;

internal sealed class TestAiInsightsService : IAiInsightsService
{
    public Task<AiInsightResultDto> BuildDailyInsightAsync(DailySummaryDto summary, CancellationToken cancellationToken)
    {
        return Task.FromResult(new AiInsightResultDto
        {
            Message = $"Completed: {summary.HabitsCompleted}; skipped: {summary.HabitsSkipped}."
        });
    }

    public Task<AiInsightResultDto> BuildHabitSupportMessageAsync(string habitName, string scenario, CancellationToken cancellationToken)
    {
        return Task.FromResult(new AiInsightResultDto { Message = $"Keep going with {habitName}." });
    }

    public Task<AiInsightResultDto> BuildCitySummaryAsync(string city, List<CityHabitStatDto> stats, CancellationToken cancellationToken)
    {
        return Task.FromResult(new AiInsightResultDto { Message = $"{city}: {stats.Count} habits." });
    }
}
