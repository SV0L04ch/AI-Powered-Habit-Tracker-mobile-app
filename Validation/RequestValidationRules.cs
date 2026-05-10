using System.Globalization;
using HabitApi.Models.Domain;

namespace HabitApi.Validation;

public static class RequestValidationRules
{
    public static bool BeValidTriggerValue(TriggerType triggerType, string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        return triggerType switch
        {
            TriggerType.CountPerDay => int.TryParse(value, out var count) && count > 0,
            TriggerType.TimeOfDay => BeValidTimeOfDay(value),
            _ => false
        };
    }

    public static bool BeValidTimeOfDay(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        return TimeOnly.TryParseExact(
            value.Trim(),
            "HH:mm",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out _);
    }

    public static bool BePastOrToday(DateOnly value)
    {
        return value <= DateOnly.FromDateTime(DateTime.UtcNow);
    }
}
