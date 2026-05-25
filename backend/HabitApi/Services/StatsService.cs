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

    public async Task<DailySummaryDto> GetDailySummaryAsync(
        Guid userId,
        DateOnly date,
        CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (user is null)
            throw new KeyNotFoundException("User not found.");

        var habitIds = await _dbContext.Habits
            .Where(h => h.UserId == userId && h.IsActive)
            .Select(h => h.Id)
            .ToListAsync(cancellationToken);

        var entries = await _dbContext.HabitEntries
            .Where(e => habitIds.Contains(e.HabitId) && e.Date == date)
            .ToListAsync(cancellationToken);

        var completed = entries.Count(e => e.Status == HabitEntryStatus.Completed);
        var partiallyCompleted = entries.Count(e => e.Status == HabitEntryStatus.Partial);
        var skipped = entries.Count(e => e.Status == HabitEntryStatus.Skipped);

        var weather = await TryGetWeatherAsync(user.City, date, cancellationToken);

        var summary = new DailySummaryDto
        {
            Date = date,
            HabitsCompleted = completed,
            HabitsPartiallyCompleted = partiallyCompleted,
            HabitsSkipped = skipped,
            Weather = weather,
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
                PopularHabits = new List<CityHabitStatDto>()
            };
        }

        var habits = await _dbContext.Habits
            .Where(h => userIds.Contains(h.UserId) && h.IsActive)
            .ToListAsync(cancellationToken);

        var habitIds = habits.Select(h => h.Id).ToHashSet();

        var entries = await _dbContext.HabitEntries
            .Where(e => habitIds.Contains(e.HabitId) && e.Date >= weekStart && e.Date <= weekEnd)
            .ToListAsync(cancellationToken);

        var completedHabitIds = entries.Select(e => e.HabitId).ToHashSet();

        var habitStats = habits
            .GroupBy(h => h.Name)
            .Select(group => new
            {
                HabitName = group.Key,
                UserCount = group
                    .Where(h => completedHabitIds.Contains(h.Id))
                    .Select(h => h.UserId)
                    .Distinct()
                    .Count()
            })
            .OrderByDescending(x => x.UserCount)
            .Take(10)
            .Select(x => new CityHabitStatDto
            {
                HabitName = x.HabitName,
                UserCount = x.UserCount,
                TotalUsers = userIds.Count
            })
            .ToList();

        return new CitySummaryDto
        {
            City = city,
            WeekStartDate = weekStart,
            WeekEndDate = weekEnd,
            PopularHabits = habitStats
        };
    }

    private async Task<WeatherSnapshotDto?> TryGetWeatherAsync(
        string city,
        DateOnly date,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(city))
            return null;

        try
        {
            return await _weatherService.GetWeatherAsync(city, date, cancellationToken);
        }
        catch (Exception ex) when (ex is KeyNotFoundException or ArgumentException or HttpRequestException or TaskCanceledException)
        {
            return new WeatherSnapshotDto
            {
                City = city,
                Date = date,
                Condition = "Weather unavailable",
                TemperatureCelsius = 0,
                HumidityPercent = null,
                Precipitation = "unknown"
            };
        }
    }
}
