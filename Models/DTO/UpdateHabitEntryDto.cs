using HabitApi.Models.Domain;

namespace HabitApi.Models.DTO;

/// <summary>
/// DTO для обновления существующей отметки выполнения привычки.
/// Все поля опциональны: можно обновлять только нужные значения.
/// </summary>
public sealed class UpdateHabitEntryDto
{
    /// <summary>
    /// Новая дата отметки.
    /// </summary>
    public DateOnly? Date { get; set; }

    /// <summary>
    /// Для положительных привычек: Completed, Partial, Skipped.
    /// Для отрицательных привычек поле игнорируется.
    /// </summary>
    public HabitEntryStatus? Status { get; set; }

    /// <summary>
    /// Для положительных привычек с типом CountPerDay — сколько выполнено.
    /// Используется, если итоговый статус равен Partial.
    /// </summary>
    public int? PartialValue { get; set; }

    /// <summary>
    /// Для отрицательных привычек — количество срывов за день.
    /// </summary>
    public int? RelapseCount { get; set; }

    /// <summary>
    /// Новая пользовательская заметка.
    /// Чтобы очистить заметку, можно передать пустую строку.
    /// </summary>
    public string? Note { get; set; }
}
