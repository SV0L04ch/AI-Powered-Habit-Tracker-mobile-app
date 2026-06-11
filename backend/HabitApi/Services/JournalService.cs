using HabitApi.Data;
using HabitApi.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace HabitApi.Services;

public interface IJournalService
{
    Task<List<HabitNote>> GetNotesAsync(Guid userId, Guid habitId);
    Task<HabitNote> AddNoteAsync(Guid userId, Guid habitId, string text, int? mood);
    Task<List<MoodEntry>> GetMoodHistoryAsync(Guid userId, int days = 30);
    Task<MoodEntry> LogMoodAsync(Guid userId, int mood, string? notes);
    Task<List<SleepEntry>> GetSleepHistoryAsync(Guid userId, int days = 30);
    Task<SleepEntry> LogSleepAsync(Guid userId, DateTime bedtime, DateTime wakeTime, int quality, string? notes);
    Task<List<MealEntry>> GetMealHistoryAsync(Guid userId, int days = 7);
    Task<MealEntry> LogMealAsync(Guid userId, string type, string foods, int? calories, string? notes);
    Task<List<Goal>> GetGoalsAsync(Guid userId);
    Task<Goal> CreateGoalAsync(Guid userId, string title, int targetValue, DateTime deadline);
    Task<Goal?> UpdateGoalAsync(Guid userId, Guid goalId, int? currentValue, bool? isCompleted);
    Task<bool> DeleteGoalAsync(Guid userId, Guid goalId);
}

public class JournalService : IJournalService
{
    private readonly AppDbContext _db;
    public JournalService(AppDbContext db) => _db = db;

    public async Task<List<HabitNote>> GetNotesAsync(Guid userId, Guid habitId)
    {
        return await _db.HabitNotes.Where(n => n.UserId == userId && n.HabitId == habitId).OrderByDescending(n => n.Date).ToListAsync();
    }

    public async Task<HabitNote> AddNoteAsync(Guid userId, Guid habitId, string text, int? mood)
    {
        var note = new HabitNote { Id = Guid.NewGuid(), HabitId = habitId, UserId = userId, Date = DateTime.UtcNow.Date, Text = text, Mood = mood, CreatedAt = DateTime.UtcNow };
        _db.HabitNotes.Add(note);
        await _db.SaveChangesAsync();
        return note;
    }

    public async Task<List<MoodEntry>> GetMoodHistoryAsync(Guid userId, int days = 30)
    {
        var since = DateTime.UtcNow.AddDays(-days);
        return await _db.MoodEntries.Where(m => m.UserId == userId && m.Date >= since).OrderByDescending(m => m.Date).ToListAsync();
    }

    public async Task<MoodEntry> LogMoodAsync(Guid userId, int mood, string? notes)
    {
        var entry = new MoodEntry { Id = Guid.NewGuid(), UserId = userId, Date = DateTime.UtcNow.Date, Mood = mood, Notes = notes, CreatedAt = DateTime.UtcNow };
        _db.MoodEntries.Add(entry);
        await _db.SaveChangesAsync();
        return entry;
    }

    public async Task<List<SleepEntry>> GetSleepHistoryAsync(Guid userId, int days = 30)
    {
        var since = DateTime.UtcNow.AddDays(-days);
        return await _db.SleepEntries.Where(s => s.UserId == userId && s.CreatedAt >= since).OrderByDescending(s => s.CreatedAt).ToListAsync();
    }

    public async Task<SleepEntry> LogSleepAsync(Guid userId, DateTime bedtime, DateTime wakeTime, int quality, string? notes)
    {
        var entry = new SleepEntry { Id = Guid.NewGuid(), UserId = userId, Bedtime = bedtime, WakeTime = wakeTime, Quality = quality, Notes = notes, CreatedAt = DateTime.UtcNow };
        _db.SleepEntries.Add(entry);
        await _db.SaveChangesAsync();
        return entry;
    }

    public async Task<List<MealEntry>> GetMealHistoryAsync(Guid userId, int days = 7)
    {
        var since = DateTime.UtcNow.AddDays(-days);
        return await _db.MealEntries.Where(m => m.UserId == userId && m.CreatedAt >= since).OrderByDescending(m => m.CreatedAt).ToListAsync();
    }

    public async Task<MealEntry> LogMealAsync(Guid userId, string type, string foods, int? calories, string? notes)
    {
        var entry = new MealEntry { Id = Guid.NewGuid(), UserId = userId, Type = type, Foods = foods, Calories = calories, Notes = notes, CreatedAt = DateTime.UtcNow };
        _db.MealEntries.Add(entry);
        await _db.SaveChangesAsync();
        return entry;
    }

    public async Task<List<Goal>> GetGoalsAsync(Guid userId)
    {
        return await _db.Goals.Where(g => g.UserId == userId).OrderByDescending(g => g.CreatedAt).ToListAsync();
    }

    public async Task<Goal> CreateGoalAsync(Guid userId, string title, int targetValue, DateTime deadline)
    {
        var goal = new Goal { Id = Guid.NewGuid(), UserId = userId, Title = title, TargetValue = targetValue, CurrentValue = 0, Deadline = deadline, CreatedAt = DateTime.UtcNow };
        _db.Goals.Add(goal);
        await _db.SaveChangesAsync();
        return goal;
    }

    public async Task<Goal?> UpdateGoalAsync(Guid userId, Guid goalId, int? currentValue, bool? isCompleted)
    {
        var goal = await _db.Goals.FirstOrDefaultAsync(g => g.Id == goalId && g.UserId == userId);
        if (goal == null) return null;

        if (currentValue.HasValue)
            goal.CurrentValue = currentValue.Value;
        if (isCompleted.HasValue)
            goal.IsCompleted = isCompleted.Value;

        if (goal.CurrentValue >= goal.TargetValue)
            goal.IsCompleted = true;

        await _db.SaveChangesAsync();
        return goal;
    }

    public async Task<bool> DeleteGoalAsync(Guid userId, Guid goalId)
    {
        var goal = await _db.Goals.FirstOrDefaultAsync(g => g.Id == goalId && g.UserId == userId);
        if (goal == null) return false;

        _db.Goals.Remove(goal);
        await _db.SaveChangesAsync();
        return true;
    }
}
