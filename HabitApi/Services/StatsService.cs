using HabitApi.Data;
using HabitApi.Models.Domain;
using HabitApi.Models.DTO;
using HabitApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HabitApi.Services;

/// <summary>
/// Сервис для формирования статистики и сводок (персональных и городских).
/// Использует данные о привычках, погоду и AI для генерации отчётов.
/// </summary>
public sealed class StatsService : IStatsService
{
    private readonly AppDbContext _dbContext;
    private readonly IWeatherService _weatherService;
    private readonly IAiInsightsService _aiInsightsService;

    /// <summary>
    /// Инициализирует сервис статистики с зависимостями базы данных, погоды и AI.
    /// </summary>
    public StatsService(
        AppDbContext dbContext,
        IWeatherService weatherService,
        IAiInsightsService aiInsightsService)
    {
        _dbContext = dbContext;
        _weatherService = weatherService;
        _aiInsightsService = aiInsightsService;
    }

    /// <inheritdoc />
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

        var completed = entries.Count(e => e.Status == HabitEntryStatus.Completed);
        var partiallyCompleted = entries.Count(e => e.Status == HabitEntryStatus.Partial);
        var skipped = entries.Count(e => e.Status == HabitEntryStatus.Skipped);

        // Получаем погоду для города пользователя на указанную дату
        var weather = await _weatherService.GetWeatherAsync(user.City, date, cancellationToken);

        var summary = new DailySummaryDto
        {
            Date = date,
            HabitsCompleted = completed,
            HabitsPartiallyCompleted = partiallyCompleted,
            HabitsSkipped = skipped,
            Weather = weather,
            AiInsight = string.Empty
        };

        // Генерируем AI-комментарий на основе сводки и погоды
        summary.AiInsight = await _aiInsightsService.BuildDailyInsightAsync(summary, cancellationToken);
        return summary;
    }

    /// <inheritdoc />
    public async Task<CitySummaryDto> GetWeeklyCitySummaryAsync(string city, CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var weekStart = today.AddDays(-(int)today.DayOfWeek + 1 - 7); // начало прошлой недели (пн)
        var weekEnd = weekStart.AddDays(6); // конец прошлой недели (вс)

        var userIds = await _dbContext.Users
            .Where(u => u.City == city)
            .Select(u => u.Id)
            .ToListAsync(cancellationToken);

        if (!userIds.Any())
            return new CitySummaryDto
            {
                City = city,
                WeekStartDate = weekStart,
                WeekEndDate = weekEnd,
                PopularHabits = new List<CityHabitStatDto>()
            };

        var habits = await _dbContext.Habits
            .Where(h => userIds.Contains(h.UserId) && h.IsActive)
            .ToListAsync(cancellationToken);

        var habitIds = habits.Select(h => h.Id).ToHashSet();

        var entries = await _dbContext.HabitEntries
            .Where(e => habitIds.Contains(e.HabitId) && e.Date >= weekStart && e.Date <= weekEnd)
            .ToListAsync(cancellationToken);

        // Группируем по названию привычки и считаем уникальных пользователей
        var habitStats = habits
            .GroupJoin(entries,
                h => h.Id,
                e => e.HabitId,
                (h, entryGroup) => new
                {
                    HabitName = h.Name,
                    UserCount = entryGroup
                        .Select(e => e.Habit?.UserId)
                        .Where(uid => uid.HasValue)
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
}
