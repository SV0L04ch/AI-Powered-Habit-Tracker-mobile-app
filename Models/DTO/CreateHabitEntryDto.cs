using HabitApi.Models.Domain;

namespace HabitApi.Models.DTO;

/// <summary>
/// DTO для создания отметки выполнения привычки.
/// </summary>
public sealed class CreateHabitEntryDto
{
    /// <summary>
    /// Дата отметки. По умолчанию сегодня.
    /// </summary>
    public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);

    /// <summary>
    /// Статус выполнения для положительных привычек: Completed, Partial, Skipped.
    /// Для отрицательных привычек игнорируется.
    /// </summary>
    public HabitEntryStatus? Status { get; set; }

    /// <summary>
    /// Количество выполненного для положительных привычек с типом CountPerDay (если Status = Partial).
    /// </summary>
    public int? PartialValue { get; set; }

    /// <summary>
    /// Количество срывов за день для отрицательных привычек (например, 3 сигареты).
    /// </summary>
    public int? RelapseCount { get; set; }

    /// <summary>
    /// Пользовательская заметка (необязательно).
    /// </summary>
    public string? Note { get; set; }
}
