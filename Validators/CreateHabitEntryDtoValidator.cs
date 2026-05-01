using FluentValidation;
using HabitApi.Models.DTO;
using HabitApi.Models.Domain;

namespace HabitApi.Validators;

public class CreateHabitEntryDtoValidator : AbstractValidator<CreateHabitEntryDto>
{
    public CreateHabitEntryDtoValidator()
    {
        RuleFor(x => x.Date)
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("Entry date cannot be in the future.");

        When(x => x.Status != null, () =>
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

        When(x => x.RelapseCount.HasValue, () =>
        {
            RuleFor(x => x.RelapseCount)
                .GreaterThan(0).WithMessage("RelapseCount must be positive.");
        });
    }
}
