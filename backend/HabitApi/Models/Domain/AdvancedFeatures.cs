namespace HabitApi.Models.Domain;

public class MoodEntry
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public DateTime Date { get; set; }
    public int Mood { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class HabitNote
{
    public Guid Id { get; set; }
    public Guid HabitId { get; set; }
    public Guid UserId { get; set; }
    public DateTime Date { get; set; }
    public string Text { get; set; } = string.Empty;
    public int? Mood { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class HabitPhoto
{
    public Guid Id { get; set; }
    public Guid HabitEntryId { get; set; }
    public Guid UserId { get; set; }
    public string PhotoUrl { get; set; } = string.Empty;
    public string? Caption { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class HabitLocation
{
    public Guid Id { get; set; }
    public Guid HabitEntryId { get; set; }
    public Guid UserId { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? Name { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class SocialFeed
{
    public Guid Id { get; set; }
    public string City { get; set; } = string.Empty;
    public string HabitName { get; set; } = string.Empty;
    public DateTime CompletedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class Friendship
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid FriendId { get; set; }
    public string Status { get; set; } = "pending";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class Challenge
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid CreatorId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class League
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Tier { get; set; } = string.Empty;
    public int MinXP { get; set; }
    public int MaxXP { get; set; }
}

public class Goal
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int TargetValue { get; set; }
    public int CurrentValue { get; set; }
    public DateTime Deadline { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class SleepEntry
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public DateTime Bedtime { get; set; }
    public DateTime WakeTime { get; set; }
    public int Quality { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class MealEntry
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Foods { get; set; } = string.Empty;
    public int? Calories { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class Webhook
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Url { get; set; } = string.Empty;
    public List<string> Events { get; set; } = new();
    public string? Secret { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
