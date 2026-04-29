using HabitApi.Models.Domain;
using System.ComponentModel.DataAnnotations;

namespace HabitApi.Models.DTO;

/// <summary>
/// DTO для обновления существующей привычки (все поля опциональны).
/// </summary>
public sealed class UpdateHabitDto
{
    [MaxLength(200)]
    public string? Name { get; set; }
    
    public bool? IsPositive { get; set; }
    
    public bool? HasPenalty { get; set; }

    public TriggerType? TriggerType { get; set; }

    public string? TriggerValue { get; set; } // "8" или "14:30"
    public int? TargetDays { get; set; }
    public int? PenaltyDaysPerMiss { get; set; }
    public List<string>? Reminders { get; set; } // "HH:mm"
}
