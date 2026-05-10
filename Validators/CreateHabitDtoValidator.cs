using FluentValidation;
using HabitApi.Models.DTO;
using HabitApi.Validation;

namespace HabitApi.Validators;

public sealed class CreateHabitDtoValidator : AbstractValidator<CreateHabitDto>
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
            .Must((dto, value) => RequestValidationRules.BeValidTriggerValue(dto.TriggerType, value))
            .WithMessage("TriggerValue format is invalid for selected TriggerType.");

        RuleForEach(x => x.Reminders)
            .Must(RequestValidationRules.BeValidTimeOfDay)
            .WithMessage("Reminder time must be in HH:mm format.");

        RuleFor(x => x)
            .Must(x => !(!x.IsPositive && (x.TargetDays != 0 || x.PenaltyDaysPerMiss != 0)))
            .WithMessage("Negative habits should not have TargetDays or PenaltyDaysPerMiss.");
    }
}
