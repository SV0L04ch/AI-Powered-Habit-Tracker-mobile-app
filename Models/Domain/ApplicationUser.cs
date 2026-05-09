using Microsoft.AspNetCore.Identity;

namespace HabitApi.Models.Domain;

public class ApplicationUser : IdentityUser<Guid>
{
    public string City { get; set; } = string.Empty;
    public string? Name { get; set; }
    public string TimeZoneId { get; set; } = "UTC";
    public bool HabitReminderEnabled { get; set; } = false;
    public string? HabitReminderTime { get; set; }
    public string ThemePreference { get; set; } = "light";
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    // Навигационное свойство для связи с Habit
    public ICollection<Habit> Habits { get; set; } = new List<Habit>();
}
