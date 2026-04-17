using HabitApi.Models.Domain;
using System.ComponentModel.DataAnnotations;

public sealed class CreateHabitDto
{
    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    public HabitType Type { get; set; } = HabitType.Positive;
    public HabitCategory Category { get; set; } = HabitCategory.Entertainment;
    public TriggerType TriggerType { get; set; } = TriggerType.CountPerDay;

    [Required]
    public string TriggerValue { get; set; } = string.Empty; // "8" или "14:30"

    public int TargetDays { get; set; } = 30;
    public int PenaltyDaysPerMiss { get; set; } = 0;
    public List<string> Reminders { get; set; } = new(); // "HH:mm"
    public List<Guid> TagIds { get; set; } = new(); // существующие теги
}
