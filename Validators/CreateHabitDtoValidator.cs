using FluentValidation;
using HabitApi.Models.DTO;
using HabitApi.Models.Domain;

namespace HabitApi.Validators;

public class CreateHabitDtoValidator : AbstractValidator<CreateHabitDto>
{
    public CreateHabitDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Habit name is required.")
            .MaximumLength(200).WithMessage("Name cannot exceed 200 characters.");

        RuleFor(x => x.TargetDays)
            .GreaterThan(0).When(x => x.IsPositive)
            .WithMessage("TargetDays must be at least 1 for positive habits.")
            .Equal(0).When(x => !x.IsPositive)
            .WithMessage("TargetDays must be 0 for negative habits.");

        RuleFor(x => x.PenaltyDaysPerMiss)
            .GreaterThanOrEqualTo(0).WithMessage("PenaltyDaysPerMiss cannot be negative.")
            .Equal(0).When(x => !x.HasPenalty)
            .WithMessage("PenaltyDaysPerMiss must be 0 for habits without penalty.");

        RuleFor(x => x.TriggerType)
            .IsInEnum().WithMessage("Invalid TriggerType.");

        RuleFor(x => x.TriggerValue)
            .NotEmpty().WithMessage("TriggerValue is required.")
            .Must((dto, value) => ValidateTriggerValue(dto.TriggerType, value))
            .WithMessage("TriggerValue format is invalid for selected TriggerType.");

        RuleForEach(x => x.Reminders)
            .Must(BeValidTime).WithMessage("Reminder time must be in HH:mm format.");

        // Дополнительная семантическая проверка: для вредных привычек не должно быть TargetDays и PenaltyDaysPerMiss
        RuleFor(x => x)
            .Must(x => !(!x.IsPositive && (x.TargetDays != 0 || x.PenaltyDaysPerMiss != 0)))
            .WithMessage("Negative habits should not have TargetDays or PenaltyDaysPerMiss.");
    }

    private bool ValidateTriggerValue(TriggerType type, string value)
    {
        if (string.IsNullOrEmpty(value)) return false;
        if (type == TriggerType.CountPerDay)
            return int.TryParse(value, out int count) && count > 0;
        if (type == TriggerType.TimeOfDay)
            return TimeSpan.TryParseExact(value, @"hh\:mm", null, out _);
        return false;
    }

    private bool BeValidTime(string time)
    {
        return TimeSpan.TryParseExact(time, @"hh\:mm", null, out _);
    }
}
