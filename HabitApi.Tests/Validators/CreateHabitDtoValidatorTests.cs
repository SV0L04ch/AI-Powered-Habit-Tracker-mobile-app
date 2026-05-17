using Xunit;
using FluentValidation.TestHelper;
using HabitApi.Models.Domain;
using HabitApi.Models.DTO;
using HabitApi.Validators;

namespace HabitApi.Tests.Validators;

/// <summary>
/// Модульные тесты для <see cref="CreateHabitDtoValidator"/>.
/// Проверяет правила валидации при создании новой привычки.
/// </summary>
public class CreateHabitDtoValidatorTests
{
    private readonly CreateHabitDtoValidator _validator = new();

    // Название
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

    // TargetDays
    [Fact]
    public void Should_Have_Error_When_TargetDays_Zero_For_Positive_Habit()
    {
        var dto = new CreateHabitDto
        {
            Name = "Test",
            IsPositive = true,
            TargetDays = 0,
            TriggerType = TriggerType.CountPerDay,
            TriggerValue = "1"
        };
        var result = _validator.Validate(dto);
        Assert.Contains(result.Errors, e => e.PropertyName == "TargetDays");
    }

    [Fact]
    public void Should_Have_Error_When_TargetDays_Not_Zero_For_Negative_Habit()
    {
        var dto = new CreateHabitDto
        {
            Name = "Test",
            IsPositive = false,
            TargetDays = 10,
            TriggerType = TriggerType.CountPerDay,
            TriggerValue = "0"
        };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.TargetDays);
    }

    // PenaltyDaysPerMiss
    [Fact]
    public void Should_Have_Error_When_PenaltyDaysPerMiss_Negative()
    {
        var dto = new CreateHabitDto
        {
            Name = "Test",
            HasPenalty = true,
            PenaltyDaysPerMiss = -1,
            TriggerType = TriggerType.CountPerDay,
            TriggerValue = "1"
        };
        var result = _validator.Validate(dto);
        Assert.Contains(result.Errors, e => e.PropertyName == "PenaltyDaysPerMiss");
    }

    [Fact]
    public void Should_Have_Error_When_Penalty_Not_Zero_Without_Penalty_Flag()
    {
        var dto = new CreateHabitDto
        {
            Name = "Test",
            HasPenalty = false,
            PenaltyDaysPerMiss = 5,
            TriggerType = TriggerType.CountPerDay,
            TriggerValue = "1"
        };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.PenaltyDaysPerMiss);
    }

    // Trigger
    [Fact]
    public void Should_Have_Error_When_TriggerType_Invalid()
    {
        var dto = new CreateHabitDto { Name = "Test", TriggerType = (TriggerType)999, TriggerValue = "1" };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.TriggerType);
    }

    [Fact]
    public void Should_Have_Error_When_TriggerValue_Empty()
    {
        var dto = new CreateHabitDto { Name = "Test", TriggerType = TriggerType.CountPerDay, TriggerValue = "" };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.TriggerValue);
    }

    [Fact]
    public void Should_Have_Error_When_TriggerValue_Invalid_For_TimeOfDay()
    {
        var dto = new CreateHabitDto { Name = "Test", TriggerType = TriggerType.TimeOfDay, TriggerValue = "abc" };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.TriggerValue);
    }

    [Fact]
    public void Should_Have_Error_When_TriggerValue_Invalid_For_CountPerDay()
    {
        var dto = new CreateHabitDto { Name = "Test", TriggerType = TriggerType.CountPerDay, TriggerValue = "-1" };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.TriggerValue);
    }

    // Reminders
    [Fact]
    public void Should_Have_Error_When_Reminder_Invalid_Format()
    {
        var dto = new CreateHabitDto
        {
            Name = "Test",
            Reminders = new List<string> { "25:00" },
            TriggerType = TriggerType.TimeOfDay,
            TriggerValue = "08:00"
        };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor("Reminders[0]");
    }

    // Полностью валидный объект
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
            HasPenalty = true,
            PenaltyDaysPerMiss = 2,
            Reminders = new List<string> { "08:00" }
        };
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
