using HabitApi.Models.DTO;
using HabitApi.Models.Domain;
using HabitApi.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.Globalization;
using Microsoft.EntityFrameworkCore;

namespace HabitApi.Services;

public sealed class ProfileService : IProfileService
{
    private const string LightTheme = "light";
    private readonly UserManager<ApplicationUser> _userManager;

    public ProfileService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<UserProfileDto?> GetProfileAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await _userManager.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);

        return user is null ? null : MapToDto(user);
    }

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

    private static string? NormalizeOptionalText(string value, int maxLength, string paramName)
    {
        var normalized = value.Trim().Normalize();
        if (string.IsNullOrWhiteSpace(normalized))
            return null;

        if (normalized.Length > maxLength)
            throw new ArgumentException($"{paramName} is too long.", paramName);

        return normalized;
    }

    private static string NormalizeRequiredText(string value, int maxLength, string paramName)
    {
        var normalized = value.Trim().Normalize();
        if (string.IsNullOrWhiteSpace(normalized))
            throw new ArgumentException($"{paramName} cannot be empty.", paramName);

        if (normalized.Length > maxLength)
            throw new ArgumentException($"{paramName} is too long.", paramName);

        return normalized;
    }

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

    private static string NormalizeThemePreference(string value)
    {
        var normalized = value.Trim().ToLowerInvariant();
        if (normalized is not (LightTheme or "dark"))
            throw new ArgumentException("Theme preference must be either 'light' or 'dark'.");

        return normalized;
    }
}
