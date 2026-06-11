namespace HabitApi.Models.Domain;

public class UserLevel
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public int XP { get; set; }
    public int Level { get; set; } = 1;
    public int NextLevelXP { get; set; } = 100;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
