using Microsoft.AspNetCore.Identity;

namespace HabitApi.Models.Domain;

/// <summary>
/// Пользователь приложения. Расширяет стандартный IdentityUser дополнительными полями профиля.
/// </summary>
public sealed class ApplicationUser : IdentityUser<Guid>
{
    /// <summary>Город пользователя (для погодной аналитики и городской сводки).</summary>
    public string City { get; set; } = string.Empty;

    /// <summary>Отображаемое имя пользователя.</summary>
    public string? Name { get; set; }

    /// <summary>Идентификатор часового пояса (например, "Europe/Moscow"). По умолчанию UTC.</summary>
    public string TimeZoneId { get; set; } = "UTC";

    /// <summary>Включены ли напоминания о привычках.</summary>
    public bool HabitReminderEnabled { get; set; } = false;

    /// <summary>Время напоминания в формате HH:mm (если HabitReminderEnabled = true).</summary>
    public string? HabitReminderTime { get; set; }

    /// <summary>Тема оформления: "light" или "dark". По умолчанию "light".</summary>
    public string ThemePreference { get; set; } = "light";

    /// <summary>Дата и время регистрации пользователя (UTC).</summary>
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    /// <summary>Коллекция привычек пользователя.</summary>
    public ICollection<Habit> Habits { get; set; } = new List<Habit>();
}
