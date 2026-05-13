using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HabitApi.Models.Domain;

/// <summary>
/// Статус выполнения для положительной привычки.
/// </summary>
public enum HabitEntryStatus
{
    /// <summary>Полностью выполнено.</summary>
    Completed = 1,
    /// <summary>Выполнено частично (требуется PartialValue).</summary>
    Partial = 2,
    /// <summary>Пропущено.</summary>
    Skipped = 3
}

/// <summary>
/// Отметка выполнения привычки за конкретный день.
/// </summary>
public sealed class HabitEntry
{
    /// <summary>Уникальный идентификатор отметки.</summary>
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>Внешний ключ на привычку.</summary>
    public Guid HabitId { get; set; }

    /// <summary>Дата, за которую ставится отметка.</summary>
    public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);

    /// <summary>
    /// Статус выполнения (для положительных привычек).
    /// Может быть null, если отметка не проставлена.
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

    /// <summary>Пользовательская заметка.</summary>
    [MaxLength(500)]
    public string? Note { get; set; }

    /// <summary>Дата и время создания записи (UTC).</summary>
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    /// <summary>Навигационное свойство к привычке.</summary>
    [ForeignKey(nameof(HabitId))]
    public Habit? Habit { get; set; }
}
