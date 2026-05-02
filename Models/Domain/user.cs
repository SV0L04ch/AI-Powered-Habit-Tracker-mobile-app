using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HabitApi.Models.Domain;

/// <summary>
/// Пользователь приложения.
/// </summary>
public sealed class User
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [EmailAddress]
    [MaxLength(256)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string City { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Name { get; set; }

    public string TimeZoneId { get; set; } = "UTC";
    public bool HabitReminderEnabled { get; set; } = false;

    [MaxLength(5)]
    public string? HabitReminderTime { get; set; }

    [Required]
    [MaxLength(10)]
    public string ThemePreference { get; set; } = "light";

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    // Навигационные свойства
    public ICollection<Habit> Habits { get; set; } = new List<Habit>();
    public ICollection<HabitEntry> HabitEntries { get; set; } = new List<HabitEntry>();
}
