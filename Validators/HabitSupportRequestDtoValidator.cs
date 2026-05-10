using FluentValidation;
using HabitApi.Models.DTO;

namespace HabitApi.Validators;

public sealed class HabitSupportRequestDtoValidator : AbstractValidator<HabitSupportRequestDto>
{
    private static readonly string[] SupportedScenarios = ["lazy", "relapse", "skip"];

    public HabitSupportRequestDtoValidator()
    {
        RuleFor(x => x.Scenario)
            .NotEmpty().WithMessage("Scenario is required.")
            .Must(BeSupportedScenario)
            .WithMessage("Scenario must be one of: lazy, relapse, skip.");
    }

    private static bool BeSupportedScenario(string scenario)
    {
        return SupportedScenarios.Contains(scenario.Trim(), StringComparer.OrdinalIgnoreCase);
    }
}
