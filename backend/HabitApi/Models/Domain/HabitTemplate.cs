namespace HabitApi.Models.Domain;

public class HabitTemplate
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public bool IsPositive { get; set; } = true;
    public bool HasPenalty { get; set; }
    public int TriggerType { get; set; } = 1;
    public int TriggerValue { get; set; } = 1;
    public int TargetDays { get; set; } = 30;
    public int InstallCount { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsSystem { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
