using HabitApi.Models.Domain;
using System.ComponentModel.DataAnnotations;

namespace HabitApi.Models.DTO;

/// <summary>
/// DTO для обновления существующей привычки.
/// </summary>
public sealed class UpdateHabitDto
{
    /// <summary>Новое название привычки (до 200 символов).</summary>
    [MaxLength(200)]
    public string? Name { get; set; }

    /// <summary>Изменить тип привычки: полезная (true) или вредная (false).</summary>
    public bool? IsPositive { get; set; }

    /// <summary>Включить или отключить штрафы за пропуски.</summary>
    public bool? HasPenalty { get; set; }

    /// <summary>Новый тип триггера: TimeOfDay или CountPerDay.</summary>
    public TriggerType? TriggerType { get; set; }

    /// <summary>Новое значение триггера (время в формате HH:mm или количество).</summary>
    public string? TriggerValue { get; set; }

    /// <summary>Новое целевое количество дней.</summary>
    public int? TargetDays { get; set; }

    /// <summary>Новое количество штрафных дней за пропуск.</summary>
    public int? PenaltyDaysPerMiss { get; set; }

    /// <summary>Новый список времён напоминаний в формате HH:mm.</summary>
    public List<string>? Reminders { get; set; }
}
