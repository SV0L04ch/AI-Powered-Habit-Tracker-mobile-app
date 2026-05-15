using Xunit;
using FluentValidation.TestHelper;
using HabitApi.Models.Domain;
using HabitApi.Models.DTO;
using HabitApi.Validators;

namespace HabitApi.Tests.Validators;

public class CreateHabitDtoValidatorTests
{
    private readonly CreateHabitDtoValidator _validator = new();

    [Fact]
    public void Should_Have_Error_When_Name_Empty()
    {
        var dto = new CreateHabitDto { Name = "" };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Should_Have_Error_When_Name_Too_Long()
    {
        var dto = new CreateHabitDto { Name = new string('A', 201) };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Should_Have_Error_When_TriggerValue_Invalid_For_TimeOfDay()
    {
        var dto = new CreateHabitDto { Name = "Test", TriggerType = TriggerType.TimeOfDay, TriggerValue = "abc" };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.TriggerValue);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Valid()
    {
        var dto = new CreateHabitDto
        {
            Name = "Полезная привычка",
            IsPositive = true,
            TriggerType = TriggerType.TimeOfDay,
            TriggerValue = "08:00",
            TargetDays = 30,
            Reminders = new List<string> { "08:00" }
        };
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }
}