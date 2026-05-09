using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HabitApi.Models.Domain;

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
    
    public bool IsPositive { get; set; } = true;   // true - полезная, false - вредная
    
    public bool HasPenalty { get; set; } = false;  // true - со штрафами, false - развлекательная

    public TriggerType TriggerType { get; set; } = TriggerType.CountPerDay;

    [Required]
    [MaxLength(10)]
    public string TriggerValue { get; set; } = string.Empty;

    public int TargetDays { get; set; } = 30;
    public int PenaltyDaysPerMiss { get; set; } = 0;
    public List<string> Reminders { get; set; } = new();
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;

    // Навигационные свойства
    [ForeignKey(nameof(UserId))]
    public User? User { get; set; }

    public ICollection<HabitEntry> Entries { get; set; } = new List<HabitEntry>();
}
