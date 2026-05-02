using HabitApi.Models.Domain;

namespace HabitApi.Models.DTO;

/// <summary>
/// DTO для ответа с отметкой выполнения.
/// </summary>
public sealed class HabitEntryDto
{
    /// <summary>
    /// Идентификатор отметки.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Идентификатор привычки.
    /// </summary>
    public Guid HabitId { get; set; }

    /// <summary>
    /// Дата отметки.
    /// </summary>
    public DateOnly Date { get; set; }

    /// <summary>
    /// Статус выполнения (для положительных привычек).
    /// </summary>
    public HabitEntryStatus? Status { get; set; }

    /// <summary>
    /// Количество выполненного (для положительных привычек с типом CountPerDay).
    /// </summary>
    public int? PartialValue { get; set; }

    /// <summary>
    /// Количество срывов (для отрицательных привычек).
    /// </summary>
    public int? RelapseCount { get; set; }

    /// <summary>
    /// Заметка пользователя.
    /// </summary>
    public string? Note { get; set; }

    /// <summary>
    /// Время создания отметки (UTC).
    /// </summary>
    public DateTime CreatedAtUtc { get; set; }
}