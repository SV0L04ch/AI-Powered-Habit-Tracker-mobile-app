using HabitApi.Models.DTO;
using HabitApi.Models.Domain;
using HabitApi.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.Globalization;
using Microsoft.EntityFrameworkCore;

namespace HabitApi.Services;

/// <summary>
/// Сервис для чтения и обновления профиля текущего пользователя.
/// </summary>
public sealed class ProfileService : IProfileService
{
    private const string LightTheme = "light";
    private readonly UserManager<ApplicationUser> _userManager;

    /// <summary>
    /// Инициализирует сервис профиля с менеджером пользователей Identity.
    /// </summary>
    /// <param name="userManager">Менеджер пользователей для доступа к данным.</param>
    public ProfileService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    /// <inheritdoc />
    public async Task<UserProfileDto?> GetProfileAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        return user is null ? null : MapToDto(user);
    }

    /// <inheritdoc />
    public async Task<UserProfileDto?> UpdateProfileAsync(
        Guid userId,
        UpdateUserProfileDto request,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null)
            return null;

        if (request.Name is not null)
            user.Name = NormalizeOptionalText(request.Name, 100, nameof(request.Name));

        if (request.City is not null)
            user.City = NormalizeRequiredText(request.City, 100, nameof(request.City));

        if (request.HabitReminderEnabled.HasValue)
            user.HabitReminderEnabled = request.HabitReminderEnabled.Value;

        if (request.HabitReminderTime is not null)
            user.HabitReminderTime = NormalizeOptionalTime(request.HabitReminderTime);

        if (request.ThemePreference is not null)
            user.ThemePreference = NormalizeThemePreference(request.ThemePreference);

        if (user.HabitReminderEnabled && string.IsNullOrWhiteSpace(user.HabitReminderTime))
            throw new ArgumentException("Habit reminder time is required when reminders are enabled.");

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Profile update failed: {errors}");
        }

        return MapToDto(user);
    }

    /// <summary>
    /// Преобразует сущность <see cref="ApplicationUser"/> в DTO профиля.
    /// </summary>
    private static UserProfileDto MapToDto(ApplicationUser user)
    {
        return new UserProfileDto
        {
            Email = user.Email ?? string.Empty,
            Name = user.Name ?? string.Empty,
            City = user.City,
            HabitReminderEnabled = user.HabitReminderEnabled,
            HabitReminderTime = user.HabitReminderTime,
            ThemePreference = user.ThemePreference
        };
    }

    /// <summary>
    /// Нормализует необязательное текстовое поле: обрезает пробелы, проверяет длину, возвращает null при пустом значении.
    /// </summary>
    private static string? NormalizeOptionalText(string value, int maxLength, string paramName)
    {
        var normalized = value.Trim().Normalize();
        if (string.IsNullOrWhiteSpace(normalized))
            return null;

        if (normalized.Length > maxLength)
            throw new ArgumentException($"{paramName} is too long.", paramName);

        return normalized;
    }

    /// <summary>
    /// Нормализует обязательное текстовое поле: обрезает пробелы, проверяет заполненность и длину.
    /// </summary>
    private static string NormalizeRequiredText(string value, int maxLength, string paramName)
    {
        var normalized = value.Trim().Normalize();
        if (string.IsNullOrWhiteSpace(normalized))
            throw new ArgumentException($"{paramName} cannot be empty.", paramName);

        if (normalized.Length > maxLength)
            throw new ArgumentException($"{paramName} is too long.", paramName);

        return normalized;
    }

    /// <summary>
    /// Нормализует время напоминания в формате HH:mm. Возвращает null для пустого значения.
    /// </summary>
    private static string? NormalizeOptionalTime(string value)
    {
        var normalized = value.Trim();
        if (string.IsNullOrWhiteSpace(normalized))
            return null;

        if (!TimeOnly.TryParseExact(
                normalized,
                "HH:mm",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out _))
        {
            throw new ArgumentException("Habit reminder time must use the HH:mm format.");
        }

        return normalized;
    }

    /// <summary>
    /// Нормализует тему оформления: приводит к нижнему регистру и проверяет допустимые значения.
    /// </summary>
    private static string NormalizeThemePreference(string value)
    {
        var normalized = value.Trim().ToLowerInvariant();
        if (normalized is not (LightTheme or "dark"))
            throw new ArgumentException("Theme preference must be either 'light' or 'dark'.");

        return normalized;
    }
}
