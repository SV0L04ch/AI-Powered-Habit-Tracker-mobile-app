using FluentValidation;
using HabitApi.Models.Domain;
using HabitApi.Models.DTO;
using HabitApi.Validation;

namespace HabitApi.Validators;

public sealed class UpdateHabitEntryDtoValidator : AbstractValidator<UpdateHabitEntryDto>
{
    public UpdateHabitEntryDtoValidator()
    {
        RuleFor(x => x.Date)
            .Must(date => date is null || RequestValidationRules.BePastOrToday(date.Value))
            .WithMessage("Entry date cannot be in the future.");

        RuleFor(x => x.Note)
            .MaximumLength(500).WithMessage("Note cannot exceed 500 characters.")
            .When(x => x.Note is not null);

        When(x => x.Status.HasValue, () =>
        {
            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("Invalid Status value.");
        });

        When(x => x.Status == HabitEntryStatus.Partial, () =>
        {
            RuleFor(x => x.PartialValue)
                .NotNull().WithMessage("PartialValue is required when status is Partial.")
                .GreaterThan(0).WithMessage("PartialValue must be positive.");
        });

        When(x => x.PartialValue.HasValue, () =>
        {
            RuleFor(x => x.PartialValue)
                .GreaterThan(0).WithMessage("PartialValue must be positive.");
        });

        When(x => x.RelapseCount.HasValue, () =>
        {
            RuleFor(x => x.RelapseCount)
                .GreaterThan(0).WithMessage("RelapseCount must be positive.");
        });
    }
}
