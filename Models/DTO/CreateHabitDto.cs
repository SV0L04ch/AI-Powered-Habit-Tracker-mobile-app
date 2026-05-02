using HabitApi.Models.Domain;
using System.ComponentModel.DataAnnotations;

namespace HabitApi.Models.DTO;

public sealed class CreateHabitDto
{
    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    
    public bool IsPositive { get; set; } = true;   // true - полезная, false - вредная
    
    public bool HasPenalty { get; set; } = false;  // true - со штрафами, false - развлекательная

    public TriggerType TriggerType { get; set; } = TriggerType.CountPerDay;

    [Required]
    public string TriggerValue { get; set; } = string.Empty; // "8" или "14:30"

    public int TargetDays { get; set; } = 30;
    public int PenaltyDaysPerMiss { get; set; } = 0;
    public List<string> Reminders { get; set; } = new(); // "HH:mm"
}
