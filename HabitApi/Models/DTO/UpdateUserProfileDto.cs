namespace HabitApi.Models.DTO;

/// <summary>
/// DTO для обновления профиля текущего пользователя.
/// </summary>
public sealed class UpdateUserProfileDto
{
    /// <summary>
    /// Новое отображаемое имя.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Новый город.
    /// </summary>
    public string? City { get; set; }

    /// <summary>
    /// Включить или отключить напоминания о привычках.
    /// </summary>
    public bool? HabitReminderEnabled { get; set; }

    /// <summary>
    /// Новое время напоминания в формате HH:mm (если напоминания включены).
    /// </summary>
    public string? HabitReminderTime { get; set; }

    /// <summary>
    /// Тема оформления: "light" или "dark".
    /// </summary>
    public string? ThemePreference { get; set; }
}
