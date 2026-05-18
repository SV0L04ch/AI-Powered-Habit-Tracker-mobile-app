using FluentValidation.TestHelper;
using HabitApi.Models.Domain;
using HabitApi.Models.DTO;
using HabitApi.Validators;
using Xunit;

namespace HabitApi.Tests.Validators;

/// <summary>
/// Модульные тесты для <see cref="UpdateHabitDtoValidator"/>.
/// Проверяет правила валидации при частичном обновлении привычки.
/// </summary>
public class UpdateHabitDtoValidatorTests
{
    private readonly UpdateHabitDtoValidator _validator = new();

    /// <summary>
    /// Имя длиннее 200 символов должно вызывать ошибку валидации.
    /// </summary>
    [Fact]
    public void Should_Have_Error_When_Name_Exceeds_Max_Length()
    {
        var dto = new UpdateHabitDto
        {
            Name = new string('A', 201),
            TriggerType = TriggerType.TimeOfDay,
            TriggerValue = "08:00"
        };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    /// <summary>
    /// Некорректный формат TriggerValue для TimeOfDay должен вызывать ошибку валидации.
    /// </summary>
    [Fact]
    public void Should_Have_Error_When_TriggerValue_Invalid_For_TimeOfDay()
    {
        var dto = new UpdateHabitDto
        {
            Name = "Habit",
            TriggerType = TriggerType.TimeOfDay,
            TriggerValue = "abc"
        };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.TriggerValue);
    }

    /// <summary>
    /// Для отрицательной привычки TargetDays не может быть больше 0.
    /// </summary>
    [Fact]
    public void Should_Have_Error_When_NegativeHabit_Has_TargetDays()
    {
        var dto = new UpdateHabitDto
        {
            Name = "Bad habit",
            IsPositive = false,
            TriggerType = TriggerType.CountPerDay,
            TriggerValue = "1",
            TargetDays = 10
        };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.TargetDays);
    }

    /// <summary>
    /// Для привычки без штрафа PenaltyDaysPerMiss должен быть равен 0.
    /// </summary>
    [Fact]
    public void Should_Have_Error_When_PenaltyDaysPerMiss_Set_Without_Penalty()
    {
        var dto = new UpdateHabitDto
        {
            Name = "Habit",
            HasPenalty = false,
            PenaltyDaysPerMiss = 5,
            TriggerType = TriggerType.CountPerDay,
            TriggerValue = "1"
        };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.PenaltyDaysPerMiss);
    }

    /// <summary>
    /// Корректно заполненный DTO (частичное обновление) не должен вызывать ошибок валидации.
    /// </summary>
    [Fact]
    public void Should_Not_Have_Error_When_Valid()
    {
        var dto = new UpdateHabitDto
        {
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

    /// <summary>
    /// Пустой объект (без изменений) не должен вызывать ошибок валидации.
    /// </summary>
    [Fact]
    public void Should_Not_Have_Error_When_Empty()
    {
        var dto = new UpdateHabitDto();
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
