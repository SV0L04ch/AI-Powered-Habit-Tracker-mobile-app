using HabitApi.Models.DTO;
using HabitApi.Services.Interfaces;

namespace HabitApi.Services;

/// <summary>
/// Сервис для генерации ИИ-сообщений (заглушка, не использует реальное LLM API).
/// В будущем заменить на вызов GroqCloud / OpenAI.
/// </summary>
public sealed class AiInsightsService : IAiInsightsService
{
    /// <inheritdoc />
    public Task<string> BuildDailyInsightAsync(DailySummaryDto summary, CancellationToken cancellationToken)
    {
        // TODO: Заменить на реальный вызов LLM API с передачей статистики и погоды.
        var text = $"Completed: {summary.HabitsCompleted}, partially: {summary.HabitsPartiallyCompleted}, " +
            $"skipped: {summary.HabitsSkipped}. Weather: {summary.Weather?.Condition}. Keep steady progress.";
        return Task.FromResult(text);
    }

    /// <inheritdoc />
    public Task<string> BuildHabitSupportMessageAsync(string habitName, string scenario, CancellationToken cancellationToken)
    {
        var normalized = scenario.Trim().ToLowerInvariant();

        var text = normalized switch
        {
            "lazy" => $"If '{habitName}' feels heavy today, do a 2-minute minimum version now to keep the streak alive.",
            "relapse" => $"Relapse on '{habitName}' happened. Note one trigger, reset the next action, and continue from this step.",
            _ => $"For '{habitName}', pick the smallest next action and execute it in the next 10 minutes."
        };

        return Task.FromResult(text);
    }

    /// <inheritdoc />
    public Task<string> BuildCitySummaryAsync(string city, List<CityHabitStatDto> stats, CancellationToken cancellationToken)
    {
        // TODO: Реализовать генерацию городской сводки через LLM.
        var topHabits = stats.Take(3).Select(s => $"{s.HabitName} ({s.Percentage:F0}%)");
        var text = $"In {city}, last week: {string.Join(", ", topHabits)}. Keep building good habits!";
        return Task.FromResult(text);
    }
}
