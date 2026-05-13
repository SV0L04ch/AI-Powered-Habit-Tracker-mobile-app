using HabitApi.Models.Domain;

namespace HabitApi.Models.DTO;

/// <summary>
/// DTO для ответа с данными привычки.
/// </summary>
public sealed class HabitDto
{
    /// <summary>Уникальный идентификатор привычки.</summary>
    public Guid Id { get; set; }

    /// <summary>Название привычки.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Полезная привычка (true) или вредная (false).</summary>
    public bool IsPositive { get; set; }

    /// <summary>Включены ли штрафы за пропуски.</summary>
    public bool HasPenalty { get; set; }

    /// <summary>Тип триггера: TimeOfDay или CountPerDay.</summary>
    public TriggerType TriggerType { get; set; }

    /// <summary>Значение триггера: время в формате HH:mm или количество.</summary>
    public string TriggerValue { get; set; } = string.Empty;

    /// <summary>Целевое количество дней (только для положительных привычек).</summary>
    public int TargetDays { get; set; }

    /// <summary>Количество штрафных дней за пропуск.</summary>
    public int PenaltyDaysPerMiss { get; set; }

    /// <summary>Список времён напоминаний в формате HH:mm.</summary>
    public List<string> Reminders { get; set; } = new();

    /// <summary>Признак активности привычки.</summary>
    public bool IsActive { get; set; }

    /// <summary>Дата и время создания привычки (UTC).</summary>
    public DateTime CreatedAtUtc { get; set; }
}
