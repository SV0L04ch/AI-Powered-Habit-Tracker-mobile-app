using FluentValidation;
using HabitApi.Models.DTO;
using HabitApi.Validation;

namespace HabitApi.Validators;

public sealed class UpdateUserProfileDtoValidator : AbstractValidator<UpdateUserProfileDto>
{
    public UpdateUserProfileDtoValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(100).WithMessage("Name is too long (max 100 characters).")
            .When(x => x.Name is not null);

        RuleFor(x => x.City)
            .Must(city => !string.IsNullOrWhiteSpace(city))
            .WithMessage("City cannot be empty when provided.")
            .MaximumLength(100).WithMessage("City name is too long (max 100 characters).")
            .When(x => x.City is not null);

        RuleFor(x => x.ThemePreference)
            .Must(themePreference => !string.IsNullOrWhiteSpace(themePreference))
            .WithMessage("Theme preference cannot be empty when provided.")
            .Must(BeValidThemePreference)
            .WithMessage("Theme preference must be either 'light' or 'dark'.")
            .When(x => x.ThemePreference is not null);

        RuleFor(x => x.HabitReminderTime)
            .MaximumLength(5).WithMessage("Habit reminder time must use the HH:mm format.")
            .Must(BeValidReminderTime)
            .WithMessage("Habit reminder time must use the HH:mm format.")
            .When(x => x.HabitReminderTime is not null && !string.IsNullOrWhiteSpace(x.HabitReminderTime));
    }

    private static bool BeValidThemePreference(string? themePreference)
    {
        if (string.IsNullOrWhiteSpace(themePreference))
            return false;

        var normalized = themePreference.Trim().ToLowerInvariant();
        return normalized is "light" or "dark";
    }

    private static bool BeValidReminderTime(string? reminderTime)
    {
        if (string.IsNullOrWhiteSpace(reminderTime))
            return true;

        return RequestValidationRules.BeValidTimeOfDay(reminderTime);
    }
}
