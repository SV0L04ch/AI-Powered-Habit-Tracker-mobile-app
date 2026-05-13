using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HabitApi.Models.Domain;

/// <summary>
/// Статус выполнения для положительной привычки.
/// </summary>
public enum HabitEntryStatus
{
    Completed = 1,  // полностью выполнено
    Partial = 2,    // выполнено частично (требуется PartialValue)
    Skipped = 3     // пропущено
}

/// <summary>
/// Отметка выполнения привычки за конкретный день.
/// </summary>
public sealed class HabitEntry
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid HabitId { get; set; }

    public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);

    /// <summary>
    /// Статус выполнения (для положительных привычек).
    /// Может быть null, если отметка ещё не проставлена.
    /// </summary>
    public HabitEntryStatus? Status { get; set; }

    /// <summary>
    /// Количество выполненного (для положительных привычек с типом CountPerDay).
    /// Например, выпито 5 стаканов из 8.
    /// </summary>
    public int? PartialValue { get; set; }

    /// <summary>
    /// Количество срывов за день (для отрицательных привычек).
    /// Например, выкурено 3 сигареты.
    /// </summary>
    public int? RelapseCount { get; set; }

    /// <summary>
    /// Пользовательская заметка.
    /// </summary>
    public string? Note { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    // Навигационные свойства
    [ForeignKey(nameof(HabitId))]
    public Habit? Habit { get; set; }
}
