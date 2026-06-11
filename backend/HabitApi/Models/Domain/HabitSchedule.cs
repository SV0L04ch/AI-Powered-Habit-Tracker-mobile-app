namespace HabitApi.Models.Domain;

public class HabitSchedule
{
    public Guid Id { get; set; }
    public Guid HabitId { get; set; }
    public string Frequency { get; set; } = "daily";
    public List<int> DaysOfWeek { get; set; } = new();
    public string? TimeOfDay { get; set; }
    public List<string> Exceptions { get; set; } = new();
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
