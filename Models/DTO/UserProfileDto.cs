namespace HabitApi.Models.DTO;

/// <summary>
/// DTO for the current user's profile settings.
/// </summary>
public sealed class UserProfileDto
{
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public bool HabitReminderEnabled { get; set; }
    public string? HabitReminderTime { get; set; }
    public string ThemePreference { get; set; } = string.Empty;
}
