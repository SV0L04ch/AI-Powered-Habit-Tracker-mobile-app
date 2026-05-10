using System.Text.Json;
using HabitApi.Data;
using HabitApi.Models.Domain;
using HabitApi.Models.DTO;
using HabitApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HabitApi.Services;

public sealed class StatsService : IStatsService
{
    private readonly AppDbContext _dbContext;
    private readonly IWeatherService _weatherService;
    private readonly IAiInsightsService _aiInsightsService;

    public StatsService(
        AppDbContext dbContext,
        IWeatherService weatherService,
        IAiInsightsService aiInsightsService)
    {
        _dbContext = dbContext;
        _weatherService = weatherService;
        _aiInsightsService = aiInsightsService;
    }

    public async Task<DailySummaryDto> GetDailySummaryAsync(Guid userId, DateOnly date, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (user is null)
            throw new KeyNotFoundException("User not found.");

        var habitIds = await _dbContext.Habits
            .Where(h => h.UserId == userId && h.IsActive)
            .Select(h => h.Id)
            .ToListAsync(cancellationToken);

        var entries = await _dbContext.HabitEntries
            .Where(e => habitIds.Contains(e.HabitId) && e.Date == date)
            .ToListAsync(cancellationToken);

        var summary = new DailySummaryDto
        {
            Date = date,
            HabitsCompleted = entries.Count(e => e.Status == HabitEntryStatus.Completed),
            HabitsPartiallyCompleted = entries.Count(e => e.Status == HabitEntryStatus.Partial),
            HabitsSkipped = entries.Count(e => e.Status == HabitEntryStatus.Skipped),
            Weather = await TryGetWeatherAsync(user.City, date, cancellationToken),
            AiInsight = string.Empty
        };

        summary.AiInsight = await _aiInsightsService.BuildDailyInsightAsync(summary, cancellationToken);
        return summary;
    }

    public async Task<CitySummaryDto> GetWeeklyCitySummaryAsync(string city, CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var weekStart = today.AddDays(-(int)today.DayOfWeek + 1 - 7);
        var weekEnd = weekStart.AddDays(6);

        var userIds = await _dbContext.Users
            .Where(u => u.City == city)
            .Select(u => u.Id)
            .ToListAsync(cancellationToken);

        if (!userIds.Any())
        {
            return new CitySummaryDto
            {
                City = city,
                WeekStartDate = weekStart,
                WeekEndDate = weekEnd,
                PopularHabits = []
            };
        }

        var habits = await _dbContext.Habits
            .Where(h => userIds.Contains(h.UserId) && h.IsActive)
            .ToListAsync(cancellationToken);

        var habitIds = habits.Select(h => h.Id).ToHashSet();

        var entries = await _dbContext.HabitEntries
            .Where(e => habitIds.Contains(e.HabitId) && e.Date >= weekStart && e.Date <= weekEnd)
            .ToListAsync(cancellationToken);

        var habitStats = habits
            .GroupJoin(
                entries,
                h => h.Id,
                e => e.HabitId,
                (habit, entryGroup) => new CityHabitStatDto
                {
                    HabitName = habit.Name,
                    UserCount = entryGroup.Any() ? 1 : 0,
                    TotalUsers = userIds.Count
                })
            .OrderByDescending(x => x.UserCount)
            .ThenBy(x => x.HabitName)
            .Take(10)
            .ToList();

        return new CitySummaryDto
        {
            City = city,
            WeekStartDate = weekStart,
            WeekEndDate = weekEnd,
            PopularHabits = habitStats
        };
    }

    public async Task<HabitWeatherInsightResponseDto> GetHabitWeatherInsightAsync(
        Guid userId,
        Guid habitId,
        DateOnly date,
        bool includePreviousDayComparison,
        CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (user is null)
            throw new KeyNotFoundException("User not found.");

        var habit = await _dbContext.Habits
            .FirstOrDefaultAsync(h => h.Id == habitId && h.UserId == userId && h.IsActive, cancellationToken);
        if (habit is null)
            throw new KeyNotFoundException("Habit not found.");

        var currentEntry = await _dbContext.HabitEntries
            .FirstOrDefaultAsync(e => e.HabitId == habitId && e.Date == date, cancellationToken);

        HabitWeatherDaySummaryDto? previousDay = null;
        if (includePreviousDayComparison)
        {
            var previousDate = date.AddDays(-1);
            var previousEntry = await _dbContext.HabitEntries
                .FirstOrDefaultAsync(e => e.HabitId == habitId && e.Date == previousDate, cancellationToken);

            previousDay = CreateHabitWeatherDaySummary(
                habit,
                previousDate,
                previousEntry,
                await TryGetWeatherAsync(user.City, previousDate, cancellationToken));
        }

        var summary = new HabitWeatherInsightResponseDto
        {
            HabitId = habit.Id,
            HabitName = habit.Name,
            IsPositive = habit.IsPositive,
            Date = date,
            CurrentDay = CreateHabitWeatherDaySummary(
                habit,
                date,
                currentEntry,
                await TryGetWeatherAsync(user.City, date, cancellationToken)),
            PreviousDay = previousDay,
            Message = string.Empty
        };

        summary.Message = await _aiInsightsService.BuildHabitWeatherInsightAsync(summary, cancellationToken);
        return summary;
    }

    private async Task<WeatherSnapshotDto?> TryGetWeatherAsync(string city, DateOnly date, CancellationToken cancellationToken)
    {
        try
        {
            return await _weatherService.GetWeatherAsync(city, date, cancellationToken);
        }
        catch (ArgumentException)
        {
            return null;
        }
        catch (KeyNotFoundException)
        {
            return null;
        }
        catch (HttpRequestException)
        {
            return null;
        }
        catch (JsonException)
        {
            return null;
        }
    }

    private static HabitWeatherDaySummaryDto CreateHabitWeatherDaySummary(
        Habit habit,
        DateOnly date,
        HabitEntry? entry,
        WeatherSnapshotDto? weather)
    {
        return new HabitWeatherDaySummaryDto
        {
            Date = date,
            HasEntry = entry is not null,
            Status = entry?.Status,
            PartialValue = entry?.PartialValue,
            RelapseCount = entry?.RelapseCount,
            Note = entry?.Note,
            Weather = weather,
            PerformanceSummary = BuildPerformanceSummary(habit, entry)
        };
    }

    private static string BuildPerformanceSummary(Habit habit, HabitEntry? entry)
    {
        if (entry is null)
        {
            return habit.IsPositive
                ? "За этот день нет записи о выполнении привычки."
                : "За этот день нет записи о количестве срывов.";
        }

        if (habit.IsPositive)
        {
            return entry.Status switch
            {
                HabitEntryStatus.Completed => "Привычка полностью выполнена.",
                HabitEntryStatus.Partial when entry.PartialValue.HasValue =>
                    $"Привычка выполнена частично, значение прогресса: {entry.PartialValue.Value}.",
                HabitEntryStatus.Partial => "Привычка выполнена частично.",
                HabitEntryStatus.Skipped => "Привычка была пропущена.",
                _ => "По привычке есть запись, но статус выполнения не указан."
            };
        }

        var relapseCount = entry.RelapseCount ?? 0;
        return relapseCount switch
        {
            0 => "Срывов не зафиксировано.",
            1 => "Зафиксирован 1 срыв.",
            _ => $"Зафиксировано {relapseCount} срывов."
        };
    }
}
