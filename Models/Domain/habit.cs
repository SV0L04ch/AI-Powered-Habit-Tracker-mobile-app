using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HabitApi.Models.Domain;

/// <summary>
/// Тип триггера привычки: по времени или по количеству раз в день.
/// </summary>
public enum TriggerType
{
    /// <summary>Привязка к конкретному времени дня (например, 20:00).</summary>
    TimeOfDay = 1,
    /// <summary>Количество повторений в течение дня (например, 8 стаканов воды).</summary>
    CountPerDay = 2
}

/// <summary>
/// Привычка пользователя.
/// </summary>
public sealed class Habit
{
    /// <summary>Уникальный идентификатор.</summary>
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>Внешний ключ на владельца привычки.</summary>
    public Guid UserId { get; set; }

    /// <summary>Название привычки (до 200 символов).</summary>
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>Полезная привычка (true) или вредная (false).</summary>
    public bool IsPositive { get; set; } = true;

    /// <summary>Включены ли штрафы за пропуски.</summary>
    public bool HasPenalty { get; set; } = false;

    /// <summary>Тип триггера: TimeOfDay или CountPerDay.</summary>
    public TriggerType TriggerType { get; set; } = TriggerType.CountPerDay;

    /// <summary>Значение триггера: время в формате HH:mm или количество.</summary>
    [Required]
    [MaxLength(10)]
    public string TriggerValue { get; set; } = string.Empty;

    /// <summary>Целевое количество дней (только для положительных привычек).</summary>
    public int TargetDays { get; set; } = 30;

    /// <summary>Количество штрафных дней за один пропуск (если HasPenalty = true).</summary>
    public int PenaltyDaysPerMiss { get; set; } = 0;

    /// <summary>Список времён напоминаний в формате HH:mm (хранится как JSON).</summary>
    public List<string> Reminders { get; set; } = new();

    /// <summary>Дата и время создания привычки (UTC).</summary>
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    /// <summary>Признак активности (false — мягкое удаление).</summary>
    public bool IsActive { get; set; } = true;

    // Навигационные свойства

    /// <summary>Владелец привычки.</summary>
    [ForeignKey(nameof(UserId))]
    public ApplicationUser? User { get; set; }

    /// <summary>Отметки выполнения этой привычки.</summary>
    public ICollection<HabitEntry> Entries { get; set; } = new List<HabitEntry>();
}
