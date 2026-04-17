using HabitApi.Models.Domain;

namespace HabitApi.Models.DTO;

/// <summary>
/// DTO для ответа с данными привычки.
/// </summary>
public sealed class HabitDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public HabitType Type { get; set; }
    public HabitCategory Category { get; set; }
    public TriggerType TriggerType { get; set; }
    public string TriggerValue { get; set; } = string.Empty; // "8" или "14:30"
    public int TargetDays { get; set; }
    public int PenaltyDaysPerMiss { get; set; }
    public List<string> Reminders { get; set; } = new(); // "HH:mm"
    public List<TagDto> Tags { get; set; } = new();      // полные теги
    public bool IsActive { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
