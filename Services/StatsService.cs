using HabitApi.Data;
using HabitApi.Models.Domain;
using HabitApi.Models.DTO;
using HabitApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HabitApi.Services;

/// <summary>
/// Сервис для формирования статистики и сводок.
/// </summary>
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

    /// <inheritdoc />
    public async Task<DailySummaryDto> GetDailySummaryAsync(Guid userId, DateOnly date, CancellationToken cancellationToken)
    {
        // Если дата не указана, берём вчерашний день
        var targetDate = date;

        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (user is null)
            throw new KeyNotFoundException("User not found.");

        // Получаем ID привычек пользователя
        var habitIds = await _dbContext.Habits
            .Where(h => h.UserId == userId && h.IsActive)
            .Select(h => h.Id)
            .ToListAsync(cancellationToken);

        // Получаем отметки за указанную дату
        var entries = await _dbContext.HabitEntries
            .Where(e => habitIds.Contains(e.HabitId) && e.Date == targetDate)
            .ToListAsync(cancellationToken);

        // Подсчёт статусов (для положительных привычек)
        int completed = entries.Count(e => e.Status == HabitEntryStatus.Completed);
        int partiallyCompleted = entries.Count(e => e.Status == HabitEntryStatus.Partial);
        int skipped = entries.Count(e => e.Status == HabitEntryStatus.Skipped);

        // Получаем погоду за targetDate (вчера, если не указано)
        var weather = await _weatherService.GetWeatherAsync(user.City, targetDate, cancellationToken);

        var summary = new DailySummaryDto
        {
            Date = targetDate,
            HabitsCompleted = completed,
            HabitsPartiallyCompleted = partiallyCompleted,
            HabitsSkipped = skipped,
            Weather = weather,
            AiInsight = string.Empty
        };

        summary.AiInsight = await _aiInsightsService.BuildDailyInsightAsync(summary, cancellationToken);
        return summary;
    }

    /// <inheritdoc />
    public async Task<CitySummaryDto> GetWeeklyCitySummaryAsync(string city, CancellationToken cancellationToken)
    {
        // Определяем диапазон последней полной недели (без учёта сегодня)
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var weekStart = today.AddDays(-(int)today.DayOfWeek + 1 - 7); // начало прошлой недели (понедельник)
        var weekEnd = weekStart.AddDays(6); // конец прошлой недели (воскресенье)

        // Находим пользователей в этом городе
        var userIds = await _dbContext.Users
            .Where(u => u.City == city)
            .Select(u => u.Id)
            .ToListAsync(cancellationToken);

        if (!userIds.Any())
            return new CitySummaryDto { City = city, WeekStartDate = weekStart, WeekEndDate = weekEnd, PopularHabits = new List<CityHabitStatDto>() };

        // Получаем привычки этих пользователей
        var habits = await _dbContext.Habits
            .Where(h => userIds.Contains(h.UserId) && h.IsActive)
            .ToListAsync(cancellationToken);

        var habitIds = habits.Select(h => h.Id).ToHashSet();

        // Получаем отметки за неделю
        var entries = await _dbContext.HabitEntries
            .Where(e => habitIds.Contains(e.HabitId) && e.Date >= weekStart && e.Date <= weekEnd)
            .ToListAsync(cancellationToken);

        // Группируем по названию привычки: считаем количество уникальных пользователей, выполнивших её хотя бы раз за неделю
        var habitStats = habits
            .GroupJoin(entries,
                h => h.Id,
                e => e.HabitId,
                (h, entryGroup) => new
                {
                    HabitName = h.Name,
                    UserCount = entryGroup.Select(e => e.Habit!.UserId).Distinct().Count()
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

        var result = new CitySummaryDto
        {
            City = city,
            WeekStartDate = weekStart,
            WeekEndDate = weekEnd,
            PopularHabits = habitStats
        };

        // Опционально: генерация текста через ИИ
        // result.SummaryText = await _aiInsightsService.BuildCitySummaryAsync(city, habitStats, cancellationToken);

        return result;
    }
}
