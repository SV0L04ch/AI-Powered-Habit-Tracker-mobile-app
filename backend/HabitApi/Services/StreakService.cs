using HabitApi.Data;
using HabitApi.Models.Domain;
using HabitApi.Models.DTO;
using HabitApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HabitApi.Services;

public interface IStreakService
{
    Task<List<StreakDto>> GetUserStreaksAsync(Guid userId);
    Task<StreakDto?> GetHabitStreakAsync(Guid userId, Guid habitId);
    Task UpdateStreakOnCompletionAsync(Guid userId, Guid habitId, DateTime completedDate);
}

public class StreakService : IStreakService
{
    private readonly AppDbContext _db;

    public StreakService(AppDbContext db) => _db = db;

    public async Task<List<StreakDto>> GetUserStreaksAsync(Guid userId)
    {
        return await _db.Streaks
            .Where(s => s.UserId == userId)
            .Join(_db.Habits, s => s.HabitId, h => h.Id, (s, h) => new StreakDto(
                s.Id, s.HabitId, h.Name, s.CurrentStreak, s.LongestStreak, s.LastCompletedDate
            ))
            .OrderByDescending(s => s.CurrentStreak)
            .ToListAsync();
    }

    public async Task<StreakDto?> GetHabitStreakAsync(Guid userId, Guid habitId)
    {
        var streak = await _db.Streaks
            .Where(s => s.UserId == userId && s.HabitId == habitId)
            .FirstOrDefaultAsync();

        if (streak == null) return null;

        var habit = await _db.Habits.FindAsync(habitId);
        return new StreakDto(streak.Id, streak.HabitId, habit?.Name ?? "", streak.CurrentStreak, streak.LongestStreak, streak.LastCompletedDate);
    }

    public async Task UpdateStreakOnCompletionAsync(Guid userId, Guid habitId, DateTime completedDate)
    {
        var streak = await _db.Streaks
            .FirstOrDefaultAsync(s => s.UserId == userId && s.HabitId == habitId);

        var today = completedDate.Date;

        if (streak == null)
        {
            streak = new Streak
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                HabitId = habitId,
                CurrentStreak = 1,
                LongestStreak = 1,
                LastCompletedDate = today,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _db.Streaks.Add(streak);
        }
        else
        {
            var lastCompleted = streak.LastCompletedDate?.Date;
            if (lastCompleted == today)
                return;

            if (lastCompleted == today.AddDays(-1))
            {
                streak.CurrentStreak++;
            }
            else
            {
                streak.CurrentStreak = 1;
            }

            streak.LongestStreak = Math.Max(streak.LongestStreak, streak.CurrentStreak);
            streak.LastCompletedDate = today;
            streak.UpdatedAt = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync();
    }
}
