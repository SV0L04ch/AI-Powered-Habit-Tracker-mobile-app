using FluentValidation;
using HabitApi.Models.DTO;
using HabitApi.Validation;

namespace HabitApi.Validators;

public sealed class UpdateHabitDtoValidator : AbstractValidator<UpdateHabitDto>
{
    public UpdateHabitDtoValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(200).When(x => x.Name is not null)
            .WithMessage("Name cannot exceed 200 characters.");

        RuleFor(x => x.TriggerValue)
            .NotEmpty().When(x => x.TriggerValue is not null)
            .WithMessage("TriggerValue cannot be empty when provided.");

        RuleFor(x => x)
            .Must(x => x.TriggerType is null || x.TriggerValue is null || RequestValidationRules.BeValidTriggerValue(x.TriggerType.Value, x.TriggerValue))
            .WithMessage("TriggerValue format is invalid for selected TriggerType.");

        RuleFor(x => x.TargetDays)
            .GreaterThan(0).When(x => x.TargetDays.HasValue && x.IsPositive == true)
            .WithMessage("TargetDays must be at least 1 for positive habits.")
            .Equal(0).When(x => x.TargetDays.HasValue && x.IsPositive == false)
            .WithMessage("TargetDays must be 0 for negative habits.");

        RuleFor(x => x.PenaltyDaysPerMiss)
            .GreaterThanOrEqualTo(0).When(x => x.PenaltyDaysPerMiss.HasValue)
            .WithMessage("PenaltyDaysPerMiss cannot be negative.")
            .Equal(0).When(x => x.PenaltyDaysPerMiss.HasValue && x.HasPenalty == false)
            .WithMessage("PenaltyDaysPerMiss must be 0 for habits without penalty.");

        RuleForEach(x => x.Reminders)
            .Must(RequestValidationRules.BeValidTimeOfDay).When(x => x.Reminders is not null)
            .WithMessage("Reminder time must be in HH:mm format.");
    }
}
