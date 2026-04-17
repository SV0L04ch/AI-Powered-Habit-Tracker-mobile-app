using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HabitApi.Models.Domain;

/// <summary>
/// Тип привычки: положительная (полезная) или отрицательная (вредная).
/// </summary>
public enum HabitType
{
    Positive = 1,
    Negative = 2
}

/// <summary>
/// Категория привычки: развлекательная (без штрафов) или со штрафами.
/// </summary>
public enum HabitCategory
{
    Entertainment = 1,  // без штрафов
    Penalty = 2         // со штрафами
}

/// <summary>
/// Тип временного триггера: конкретное время дня или количество раз в день.
/// </summary>
public enum TriggerType
{
    TimeOfDay = 1,      // привязка ко времени (например, 20:00)
    CountPerDay = 2     // количество раз в день (например, 8 стаканов воды)
}

/// <summary>
/// Привычка пользователя.
/// </summary>
public sealed class Habit
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid UserId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    public HabitType Type { get; set; } = HabitType.Positive;
    public HabitCategory Category { get; set; } = HabitCategory.Entertainment;

    public TriggerType TriggerType { get; set; } = TriggerType.CountPerDay;

    /// <summary>
    /// Значение триггера: если TriggerType = TimeOfDay, то хранит время в формате "HH:mm",
    /// если CountPerDay – количество раз (целое число).
    /// Храним как строку для универсальности, либо два отдельных поля.
    /// </summary>
    [Required]
    [MaxLength(10)]
    public string TriggerValue { get; set; } = string.Empty;

    /// <summary>
    /// Целевое количество дней для достижения привычки (например, "30 дней без пропусков").
    /// </summary>
    public int TargetDays { get; set; } = 30;

    /// <summary>
    /// Количество штрафных дней, добавляемых за один пропуск (для категории Penalty).
    /// </summary>
    public int PenaltyDaysPerMiss { get; set; } = 0;

    /// <summary>
    /// Напоминания (список времени в формате "HH:mm").
    /// В реляционной БД лучше вынести в отдельную таблицу, но для простоты можно хранить как JSON.
    /// </summary>
    public List<string> Reminders { get; set; } = new();

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;

    // Навигационные свойства
    [ForeignKey(nameof(UserId))]
    public User? User { get; set; }

    public ICollection<HabitEntry> Entries { get; set; } = new List<HabitEntry>();

    // Теги – через отдельную таблицу (не List<string>)
    public ICollection<HabitTag> HabitTags { get; set; } = new List<HabitTag>();
}
