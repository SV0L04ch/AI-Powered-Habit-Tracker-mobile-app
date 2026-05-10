using FluentValidation;
using HabitApi.Models.DTO;
using HabitApi.Validation;

namespace HabitApi.Validators;

public sealed class HabitWeatherInsightRequestDtoValidator : AbstractValidator<HabitWeatherInsightRequestDto>
{
    public HabitWeatherInsightRequestDtoValidator()
    {
        RuleFor(x => x.Date)
            .Must(date => date is null || RequestValidationRules.BePastOrToday(date.Value))
            .WithMessage("Insight date cannot be in the future.");
    }
}
