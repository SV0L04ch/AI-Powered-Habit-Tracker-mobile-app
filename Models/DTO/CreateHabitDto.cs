using HabitApi.Models.Domain;
using System.ComponentModel.DataAnnotations;

namespace HabitApi.Models.DTO;

/// <summary>
/// DTO для создания новой привычки.
/// </summary>
public sealed class CreateHabitDto
{
    /// <summary>
    /// Название привычки (до 200 символов).
    /// </summary>
    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Положительная привычка (true) или вредная (false).
    /// </summary>
    public bool IsPositive { get; set; } = true;

    /// <summary>
    /// Включены ли штрафы за пропуски.
    /// </summary>
    public bool HasPenalty { get; set; } = false;

    /// <summary>
    /// Тип триггера: TimeOfDay или CountPerDay.
    /// </summary>
    public TriggerType TriggerType { get; set; } = TriggerType.CountPerDay;

    /// <summary>
    /// Значение триггера: время в формате HH:mm или количество.
    /// </summary>
    [Required]
    public string TriggerValue { get; set; } = string.Empty;

    /// <summary>
    /// Целевое количество дней (только для положительных привычек).
    /// </summary>
    public int TargetDays { get; set; } = 30;

    /// <summary>
    /// Количество штрафных дней за пропуск.
    /// </summary>
    public int PenaltyDaysPerMiss { get; set; } = 0;

    /// <summary>
    /// Список времён напоминаний в формате HH:mm.
    /// </summary>
    public List<string> Reminders { get; set; } = new();
}
