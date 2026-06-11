using HabitApi.Data;
using HabitApi.Models.Domain;
using HabitApi.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace HabitApi.Services;

public interface IGamificationService
{
    Task<GamificationDto> GetGamificationAsync(Guid userId);
    Task AddXPAsync(Guid userId, int amount, string reason);
    Task<AchievementDto?> CheckAndGrantAchievementsAsync(Guid userId);
}

public class GamificationService : IGamificationService
{
    private readonly AppDbContext _db;

    private static readonly Dictionary<string, (string Name, string Desc, string Icon)> AchievementDefs = new()
    {
        ["first_step"] = ("First Step", "Complete your first habit", "🎯"),
        ["week_warrior"] = ("Week Warrior", "7-day streak", "🔥"),
        ["month_master"] = ("Month Master", "30-day streak", "👑"),
        ["centurion"] = ("Centurion", "100-day streak", "💎"),
        ["habit_builder"] = ("Habit Builder", "Create 5 habits", "🏗️"),
        ["perfect_week"] = ("Perfect Week", "Complete all habits for 7 days", "⭐"),
        ["level_5"] = ("Rising Star", "Reach level 5", "🌟"),
        ["level_10"] = ("Dedicated", "Reach level 10", "🏅"),
    };

    private static readonly int[] LevelThresholds = [0, 100, 250, 500, 800, 1200, 1700, 2300, 3000, 3800, 4700, 5700, 6800, 8000, 9300];

    public GamificationService(AppDbContext db) => _db = db;

    public async Task<GamificationDto> GetGamificationAsync(Guid userId)
    {
        var level = await _db.UserLevels.FirstOrDefaultAsync(u => u.UserId == userId);
        if (level == null)
        {
            level = new UserLevel { Id = Guid.NewGuid(), UserId = userId, XP = 0, Level = 1, NextLevelXP = 100 };
            _db.UserLevels.Add(level);
            await _db.SaveChangesAsync();
        }

        var achievements = await _db.Achievements
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.EarnedAt)
            .Take(5)
            .Select(a => new AchievementDto(a.Id, a.Type, a.Name, a.Description, a.Icon, a.EarnedAt))
            .ToListAsync();

        var currentThreshold = level.Level < LevelThresholds.Length ? LevelThresholds[level.Level - 1] : 0;
        var nextThreshold = level.Level < LevelThresholds.Length ? LevelThresholds[level.Level] : currentThreshold + 1000;
        var progress = nextThreshold > currentThreshold
            ? (int)((level.XP - currentThreshold) * 100.0 / (nextThreshold - currentThreshold))
            : 100;

        return new GamificationDto(level.XP, level.Level, nextThreshold, progress, achievements);
    }

    public async Task AddXPAsync(Guid userId, int amount, string reason)
    {
        var level = await _db.UserLevels.FirstOrDefaultAsync(u => u.UserId == userId);
        if (level == null)
        {
            level = new UserLevel { Id = Guid.NewGuid(), UserId = userId, XP = 0, Level = 1, NextLevelXP = 100 };
            _db.UserLevels.Add(level);
        }

        level.XP += amount;
        level.UpdatedAt = DateTime.UtcNow;

        while (level.Level < LevelThresholds.Length && level.XP >= LevelThresholds[level.Level])
        {
            level.Level++;
            level.NextLevelXP = level.Level < LevelThresholds.Length ? LevelThresholds[level.Level] : level.XP + 1000;
        }

        await _db.SaveChangesAsync();
    }

    public async Task<AchievementDto?> CheckAndGrantAchievementsAsync(Guid userId)
    {
        var existingTypes = await _db.Achievements
            .Where(a => a.UserId == userId)
            .Select(a => a.Type)
            .ToListAsync();

        var streakCount = await _db.Streaks.Where(s => s.UserId == userId).MaxAsync(s => (int?)s.CurrentStreak) ?? 0;
        var habitCount = await _db.Habits.Where(h => h.UserId == userId && h.IsActive).CountAsync();
        var level = await _db.UserLevels.FirstOrDefaultAsync(u => u.UserId == userId);

        var newAchievementType = new List<string>();

        if (streakCount >= 1 && !existingTypes.Contains("first_step")) newAchievementType.Add("first_step");
        if (streakCount >= 7 && !existingTypes.Contains("week_warrior")) newAchievementType.Add("week_warrior");
        if (streakCount >= 30 && !existingTypes.Contains("month_master")) newAchievementType.Add("month_master");
        if (streakCount >= 100 && !existingTypes.Contains("centurion")) newAchievementType.Add("centurion");
        if (habitCount >= 5 && !existingTypes.Contains("habit_builder")) newAchievementType.Add("habit_builder");
        if (level != null && level.Level >= 5 && !existingTypes.Contains("level_5")) newAchievementType.Add("level_5");
        if (level != null && level.Level >= 10 && !existingTypes.Contains("level_10")) newAchievementType.Add("level_10");

        if (newAchievementType.Count == 0) return null;

        var type = newAchievementType.First();
        var def = AchievementDefs[type];
        var achievement = new Achievement
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Type = type,
            Name = def.Name,
            Description = def.Desc,
            Icon = def.Icon,
            EarnedAt = DateTime.UtcNow
        };

        _db.Achievements.Add(achievement);
        await _db.SaveChangesAsync();

        return new AchievementDto(achievement.Id, achievement.Type, achievement.Name, achievement.Description, achievement.Icon, achievement.EarnedAt);
    }
}
