using Xunit;
using FluentValidation.TestHelper;
using HabitApi.Models.Domain;
using HabitApi.Models.DTO;
using HabitApi.Validators;

namespace HabitApi.Tests.Validators;

/// <summary>
/// Модульные тесты для <see cref="CreateHabitEntryDtoValidator"/>.
/// Проверяет правила валидации при создании отметки выполнения привычки.
/// </summary>
public class CreateHabitEntryDtoValidatorTests
{
    private readonly CreateHabitEntryDtoValidator _validator = new();

    // Дата
    /// <summary>
    /// Дата отметки не может быть в будущем.
    /// </summary>
    [Fact]
    public void Should_Have_Error_When_Date_In_Future()
    {
        var dto = new CreateHabitEntryDto { Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)) };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Date);
    }

    /// <summary>
    /// Сегодняшняя дата допустима.
    /// </summary>
    [Fact]
    public void Should_Not_Have_Error_When_Date_Today()
    {
        var dto = new CreateHabitEntryDto { Date = DateOnly.FromDateTime(DateTime.UtcNow) };
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveValidationErrorFor(x => x.Date);
    }

    // Статус
    /// <summary>
    /// Статус должен быть допустимым значением перечисления <see cref="HabitEntryStatus"/>.
    /// </summary>
    [Fact]
    public void Should_Have_Error_When_Status_Invalid()
    {
        var dto = new CreateHabitEntryDto { Status = (HabitEntryStatus)999 };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Status);
    }

    /// <summary>
    /// PartialValue обязательно, если статус Partial.
    /// </summary>
    [Fact]
    public void Should_Have_Error_When_Partial_Status_Missing_PartialValue()
    {
        var dto = new CreateHabitEntryDto { Status = HabitEntryStatus.Partial, PartialValue = null };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.PartialValue);
    }

    /// <summary>
    /// PartialValue должно быть положительным числом.
    /// </summary>
    [Fact]
    public void Should_Have_Error_When_PartialValue_Zero_Or_Negative()
    {
        var dto = new CreateHabitEntryDto { Status = HabitEntryStatus.Partial, PartialValue = 0 };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.PartialValue);
    }

    // Количество срывов
    /// <summary>
    /// RelapseCount должно быть положительным числом, если указано.
    /// </summary>
    [Fact]
    public void Should_Have_Error_When_RelapseCount_Negative()
    {
        var dto = new CreateHabitEntryDto { RelapseCount = -1 };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.RelapseCount);
    }

    /// <summary>
    /// RelapseCount может отсутствовать (null).
    /// </summary>
    [Fact]
    public void Should_Not_Have_Error_When_RelapseCount_Null()
    {
        var dto = new CreateHabitEntryDto { RelapseCount = null };
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveValidationErrorFor(x => x.RelapseCount);
    }

    // Полностью валидный объект
    /// <summary>
    /// Корректно заполненный DTO (положительная привычка, Completed) не должен вызывать ошибок.
    /// </summary>
    [Fact]
    public void Should_Not_Have_Error_When_Valid_Positive_Completed()
    {
        var dto = new CreateHabitEntryDto
        {
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            Status = HabitEntryStatus.Completed,
            Note = "Всё сделано"
        };
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }

    /// <summary>
    /// Корректно заполненный DTO (отрицательная привычка, срыв) не должен вызывать ошибок.
    /// </summary>
    [Fact]
    public void Should_Not_Have_Error_When_Valid_Negative_Relapse()
    {
        var dto = new CreateHabitEntryDto
        {
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            RelapseCount = 3
        };
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
