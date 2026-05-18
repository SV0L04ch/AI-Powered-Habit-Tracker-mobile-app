using FluentValidation;
using HabitApi.Models.DTO;
using HabitApi.Models.Domain;

namespace HabitApi.Validators;

/// <summary>
/// Валидатор запроса на создание привычки.
/// Проверяет название, целевое количество дней, штрафы, тип и значение триггера, а также время напоминаний.
/// </summary>
public sealed class CreateHabitDtoValidator : AbstractValidator<CreateHabitDto>
{
    public CreateHabitDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Habit name is required.")
            .MaximumLength(200).WithMessage("Name cannot exceed 200 characters.");

        // TargetDays
        RuleFor(x => x.TargetDays)
            .Must((dto, val) => !(dto.IsPositive && val <= 0))
            .WithMessage("TargetDays must be at least 1 for positive habits.")
            .Must((dto, val) => !(!dto.IsPositive && val != 0))
            .WithMessage("TargetDays must be 0 for negative habits.");

        // PenaltyDaysPerMiss
        RuleFor(x => x.PenaltyDaysPerMiss)
            .Must((dto, val) => val >= 0)
            .WithMessage("PenaltyDaysPerMiss cannot be negative.")
            .Must((dto, val) => !(!dto.HasPenalty && val != 0))
            .WithMessage("PenaltyDaysPerMiss must be 0 for habits without penalty.");

        RuleFor(x => x.TriggerType)
            .IsInEnum().WithMessage("Invalid TriggerType.");

        RuleFor(x => x.TriggerValue)
            .NotEmpty().WithMessage("TriggerValue is required.")
            .Must((dto, value) => ValidateTriggerValue(dto.TriggerType, value))
            .WithMessage("TriggerValue format is invalid for selected TriggerType.");

        RuleForEach(x => x.Reminders)
            .Must(BeValidTime).WithMessage("Reminder time must be in HH:mm format.");

        RuleFor(x => x)
            .Must(x => !(!x.IsPositive && (x.TargetDays != 0 || x.PenaltyDaysPerMiss != 0)))
            .WithMessage("Negative habits should not have TargetDays or PenaltyDaysPerMiss.");
    }

    /// <summary>
    /// Проверяет, соответствует ли значение триггера выбранному типу.
    /// </summary>
    /// <param name="type">Тип триггера (TimeOfDay или CountPerDay).</param>
    /// <param name="value">Значение триггера (время или количество).</param>
    /// <returns>True, если значение корректно для указанного типа.</returns>
    private static bool ValidateTriggerValue(TriggerType type, string value)
    {
        if (string.IsNullOrEmpty(value)) return false;
        if (type == TriggerType.CountPerDay)
            return int.TryParse(value, out var count) && count > 0;
        if (type == TriggerType.TimeOfDay)
            return TimeSpan.TryParseExact(value, @"hh\:mm", null, out _);
        return false;
    }

    /// <summary>
    /// Проверяет, что время указано в формате HH:mm.
    /// </summary>
    private static bool BeValidTime(string time)
    {
        return TimeSpan.TryParseExact(time, @"hh\:mm", null, out _);
    }
}
