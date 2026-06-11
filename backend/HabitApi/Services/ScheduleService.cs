using HabitApi.Data;
using HabitApi.Models.Domain;
using HabitApi.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace HabitApi.Services;

public interface IScheduleService
{
    Task<HabitSchedule?> GetScheduleAsync(Guid userId, Guid habitId);
    Task<HabitSchedule> UpsertScheduleAsync(Guid userId, Guid habitId, string frequency, List<int> daysOfWeek, string? timeOfDay);
    Task<List<Habit>> GetTodayHabitsAsync(Guid userId);
}

public class ScheduleService : IScheduleService
{
    private readonly AppDbContext _db;

    public ScheduleService(AppDbContext db) => _db = db;

    public async Task<HabitSchedule?> GetScheduleAsync(Guid userId, Guid habitId)
    {
        return await _db.HabitSchedules
            .Where(s => s.HabitId == habitId)
            .FirstOrDefaultAsync();
    }

    public async Task<HabitSchedule> UpsertScheduleAsync(Guid userId, Guid habitId, string frequency, List<int> daysOfWeek, string? timeOfDay)
    {
        var schedule = await _db.HabitSchedules.FirstOrDefaultAsync(s => s.HabitId == habitId);

        if (schedule == null)
        {
            schedule = new HabitSchedule
            {
                Id = Guid.NewGuid(),
                HabitId = habitId,
                Frequency = frequency,
                DaysOfWeek = daysOfWeek,
                TimeOfDay = timeOfDay,
                CreatedAt = DateTime.UtcNow
            };
            _db.HabitSchedules.Add(schedule);
        }
        else
        {
            schedule.Frequency = frequency;
            schedule.DaysOfWeek = daysOfWeek;
            schedule.TimeOfDay = timeOfDay;
        }

        await _db.SaveChangesAsync();
        return schedule;
    }

    public async Task<List<Habit>> GetTodayHabitsAsync(Guid userId)
    {
        var today = DateTime.UtcNow.Date;
        var dayOfWeek = (int)today.DayOfWeek;

        var habits = await _db.Habits
            .Where(h => h.UserId == userId && h.IsActive)
            .ToListAsync();

        var scheduled = new List<Habit>();
        foreach (var habit in habits)
        {
            var schedule = await _db.HabitSchedules.FirstOrDefaultAsync(s => s.HabitId == habit.Id);
            if (schedule == null || !schedule.IsActive)
            {
                scheduled.Add(habit);
                continue;
            }

            var isScheduled = schedule.Frequency switch
            {
                "daily" => true,
                "weekdays" => dayOfWeek >= 1 && dayOfWeek <= 5,
                "custom" => schedule.DaysOfWeek.Contains(dayOfWeek),
                _ => true
            };

            if (isScheduled && !schedule.Exceptions.Contains(today.ToString("yyyy-MM-dd")))
                scheduled.Add(habit);
        }

        return scheduled;
    }
}
