using HabitApi.Models.Domain;
using System.ComponentModel.DataAnnotations;

namespace HabitApi.Models.DTO;

/// <summary>
/// DTO для создания отметки выполнения привычки.
/// </summary>
public sealed class CreateHabitEntryDto
{
    /// <summary>
    /// Дата отметки. По умолчанию сегодня.
    /// </summary>
    [Required]
    public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);

    /// <summary>
    /// Статус выполнения для положительных привычек: Completed, Partial, Skipped.
    /// Для отрицательных привычек игнорируется.
    /// </summary>
    public HabitEntryStatus? Status { get; set; }

    /// <summary>
    /// Количество выполненного для положительных привычек с типом CountPerDay (если Status = Partial).
    /// </summary>
    [Range(0, int.MaxValue)]
    public int? PartialValue { get; set; }

    /// <summary>
    /// Количество срывов за день для отрицательных привычек (например, 3 сигареты).
    /// </summary>
    [Range(0, int.MaxValue)]
    public int? RelapseCount { get; set; }

    /// <summary>
    /// Пользовательская заметка (необязательно).
    /// </summary>
    [MaxLength(500)]
    public string? Note { get; set; }
}
