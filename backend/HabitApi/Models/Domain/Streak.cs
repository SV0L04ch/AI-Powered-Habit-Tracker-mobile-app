namespace HabitApi.Models.Domain;

public class Streak
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid HabitId { get; set; }
    public int CurrentStreak { get; set; }
    public int LongestStreak { get; set; }
    public DateTime? LastCompletedDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
