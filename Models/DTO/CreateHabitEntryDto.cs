using HabitApi.Models.Domain;
using System.ComponentModel.DataAnnotations;

namespace HabitApi.Models.DTO;

/// <summary>
/// DTO для создания отметки выполнения привычки.
/// </summary>
public sealed class CreateHabitEntryDto
{
    [Required]
    public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);

    /// <summary>
    /// Для положительных привычек: Completed, Partial, Skipped.
    /// Для отрицательных привычек это поле игнорируется.
    /// </summary>
    public HabitEntryStatus? Status { get; set; }

    /// <summary>
    /// Для положительных привычек с типом CountPerDay – сколько выполнено (если Status = Partial).
    /// </summary>
    [Range(0, int.MaxValue)]
    public int? PartialValue { get; set; }

    /// <summary>
    /// Для отрицательных привычек – количество срывов за день (например, 3 сигареты).
    /// </summary>
    [Range(0, int.MaxValue)]
    public int? RelapseCount { get; set; }

    [MaxLength(500)]
    public string? Note { get; set; }
}

/// <summary>
/// DTO для ответа с отметкой выполнения.
/// </summary>
public sealed class HabitEntryDto
{
    public Guid Id { get; set; }
    public Guid HabitId { get; set; }
    public DateOnly Date { get; set; }
    public HabitEntryStatus? Status { get; set; }
    public int? PartialValue { get; set; }
    public int? RelapseCount { get; set; }
    public string? Note { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
