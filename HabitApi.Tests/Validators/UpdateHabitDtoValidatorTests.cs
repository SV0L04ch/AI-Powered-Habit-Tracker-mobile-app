using FluentValidation.TestHelper;
using HabitApi.Models.Domain;
using HabitApi.Models.DTO;
using HabitApi.Validators;
using Moq;
using Xunit;

namespace HabitApi.Tests.Validators;

public class UpdateHabitDtoValidatorTests
{
    private readonly UpdateHabitDtoValidator _validator = new();

    [Fact]
    public void Should_Have_Error_When_Id_Is_Empty()
    {
        var dto = new UpdateHabitDto
        {
            Id = Guid.Empty,
            Name = "Test",
            TriggerType = TriggerType.CountPerDay,
            TriggerValue = "1"
        };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Should_Have_Error_When_Name_Is_Empty()
    {
        var dto = new UpdateHabitDto
        {
            Id = Guid.NewGuid(),
            Name = "",
            TriggerType = TriggerType.CountPerDay,
            TriggerValue = "1"
        };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Should_Have_Error_When_Name_Exceeds_Max_Length()
    {
        var dto = new UpdateHabitDto
        {
            Id = Guid.NewGuid(),
            Name = new string('A', 201),
            TriggerType = TriggerType.TimeOfDay,
            TriggerValue = "08:00"
        };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Should_Have_Error_When_TriggerValue_Invalid_For_TimeOfDay()
    {
        var dto = new UpdateHabitDto
        {
            Id = Guid.NewGuid(),
            Name = "Habit",
            TriggerType = TriggerType.TimeOfDay,
            TriggerValue = "abc"
        };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.TriggerValue);
    }

    [Fact]
    public void Should_Have_Error_When_NegativeHabit_Has_TargetDays()
    {
        var dto = new UpdateHabitDto
        {
            Id = Guid.NewGuid(),
            Name = "Bad habit",
            IsPositive = false,
            TriggerType = TriggerType.CountPerDay,
            TriggerValue = "1",
            TargetDays = 10,
            PenaltyDaysPerMiss = 5
        };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor("TargetDays");
    }

    [Fact]
    public void Should_Not_Have_Error_When_Valid()
    {
        var dto = new UpdateHabitDto
        {
            Id = Guid.NewGuid(),
            Name = "Чтение",
            IsPositive = true,
            TriggerType = TriggerType.TimeOfDay,
            TriggerValue = "08:00",
            TargetDays = 30,
            PenaltyDaysPerMiss = 0,
            Reminders = new List<string> { "08:00" }
        };
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }
}