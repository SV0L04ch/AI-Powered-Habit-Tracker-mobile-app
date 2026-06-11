namespace HabitApi.Models.DTO;

public record StreakDto(
    Guid Id,
    Guid HabitId,
    string HabitName,
    int CurrentStreak,
    int LongestStreak,
    DateTime? LastCompletedDate
);

public record GamificationDto(
    int TotalXP,
    int Level,
    int NextLevelXP,
    int ProgressPercent,
    List<AchievementDto> RecentAchievements
);

public record AchievementDto(
    Guid Id,
    string Type,
    string Name,
    string Description,
    string Icon,
    DateTime EarnedAt
);

public record HabitTemplateDto(
    Guid Id,
    string Name,
    string Description,
    string Category,
    string Icon,
    bool IsPositive,
    int InstallCount
);

public record QuoteDto(
    Guid Id,
    string Text,
    string Author,
    string Category
);
