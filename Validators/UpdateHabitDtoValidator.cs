using FluentValidation;
using HabitApi.Models.DTO;
using HabitApi.Models.Domain;

namespace HabitApi.Validators;

public class UpdateHabitDtoValidator : AbstractValidator<UpdateHabitDto>
{
    public UpdateHabitDtoValidator()
    {
        // Name – если указан, проверяем длину
        RuleFor(x => x.Name)
            .MaximumLength(200).When(x => x.Name != null)
            .WithMessage("Name cannot exceed 200 characters.");

        // TriggerValue – если указан, проверяем формат в зависимости от TriggerType
        RuleFor(x => x.TriggerValue)
            .Must((dto, value) => value == null || ValidateTriggerValue(dto.TriggerType ?? TriggerType.CountPerDay, value))
            .WithMessage("TriggerValue format is invalid for selected TriggerType.");

        // TargetDays – если указан, проверяем с учётом IsPositive (если IsPositive тоже указан)
        RuleFor(x => x.TargetDays)
            .GreaterThan(0).When(x => x.TargetDays.HasValue && x.IsPositive == true)
            .WithMessage("TargetDays must be at least 1 for positive habits.")
            .Equal(0).When(x => x.TargetDays.HasValue && x.IsPositive == false)
            .WithMessage("TargetDays must be 0 for negative habits.");

        // PenaltyDaysPerMiss – если указан, проверяем неотрицательность
        RuleFor(x => x.PenaltyDaysPerMiss)
            .GreaterThanOrEqualTo(0).When(x => x.PenaltyDaysPerMiss.HasValue)
            .WithMessage("PenaltyDaysPerMiss cannot be negative.")
            .Equal(0).When(x => x.PenaltyDaysPerMiss.HasValue && x.HasPenalty == false)
            .WithMessage("PenaltyDaysPerMiss must be 0 for habits without penalty.");

        // Reminders – если указаны, проверяем каждый элемент
        RuleForEach(x => x.Reminders)
            .Must(BeValidTime).When(x => x.Reminders != null)
            .WithMessage("Reminder time must be in HH:mm format.");
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
