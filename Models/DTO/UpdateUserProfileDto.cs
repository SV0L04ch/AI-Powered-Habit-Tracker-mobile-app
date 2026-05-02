namespace HabitApi.Models.DTO;

/// <summary>
/// DTO for updating the current user's profile settings.
/// All fields are optional; omitted values are left unchanged.
/// </summary>
public sealed class UpdateUserProfileDto
{
    public string? Name { get; set; }
    public string? City { get; set; }
    public bool? HabitReminderEnabled { get; set; }
    public string? HabitReminderTime { get; set; }
    public string? ThemePreference { get; set; }
}
