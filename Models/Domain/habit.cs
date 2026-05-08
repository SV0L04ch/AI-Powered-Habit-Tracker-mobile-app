using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HabitApi.Models.Domain; // Подразумевается, что ApplicationUser здесь

namespace HabitApi.Models.Domain;

public enum TriggerType
{
    TimeOfDay = 1,
    CountPerDay = 2
}

public sealed class Habit
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid UserId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    public bool IsPositive { get; set; } = true;
    public bool HasPenalty { get; set; } = false;

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
    public ApplicationUser? User { get; set; }

    public ICollection<HabitEntry> Entries { get; set; } = new List<HabitEntry>();
}
