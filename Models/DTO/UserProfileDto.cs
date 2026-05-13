namespace HabitApi.Models.DTO;

/// <summary>
/// DTO с настройками профиля текущего пользователя.
/// </summary>
public sealed class UserProfileDto
{
    /// <summary>
    /// Электронная почта пользователя.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Отображаемое имя.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Город пользователя.
    /// </summary>
    public string City { get; set; } = string.Empty;

    /// <summary>
    /// Включены ли напоминания о привычках.
    /// </summary>
    public bool HabitReminderEnabled { get; set; }

    /// <summary>
    /// Время напоминания в формате HH:mm (если напоминания включены).
    /// </summary>
    public string? HabitReminderTime { get; set; }

    /// <summary>
    /// Тема оформления: "light" или "dark".
    /// </summary>
    public string ThemePreference { get; set; } = string.Empty;
}
