using Xunit;
using FluentValidation.TestHelper;
using HabitApi.Models.DTO;
using HabitApi.Validators;

namespace HabitApi.Tests.Validators;

/// <summary>
/// Модульные тесты для <see cref="UpdateUserProfileDtoValidator"/>.
/// Проверяет правила валидации при обновлении профиля пользователя.
/// </summary>
public class UpdateUserProfileDtoValidatorTests
{
    private readonly UpdateUserProfileDtoValidator _validator = new();

    // Имя
    /// <summary>
    /// Имя длиннее 100 символов должно вызывать ошибку валидации.
    /// </summary>
    [Fact]
    public void Should_Have_Error_When_Name_Too_Long()
    {
        var dto = new UpdateUserProfileDto { Name = new string('A', 101) };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    // Город
    /// <summary>
    /// Город длиннее 100 символов должен вызывать ошибку валидации.
    /// </summary>
    [Fact]
    public void Should_Have_Error_When_City_Too_Long()
    {
        var dto = new UpdateUserProfileDto { City = new string('A', 101) };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.City);
    }

    /// <summary>
    /// Если город передан, он не должен быть пустой строкой.
    /// </summary>
    [Fact]
    public void Should_Have_Error_When_City_Empty()
    {
        var dto = new UpdateUserProfileDto { City = "" };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.City);
    }

    // Тема оформления
    /// <summary>
    /// Тема оформления должна быть "light" или "dark".
    /// </summary>
    [Theory]
    [InlineData("blue")]
    [InlineData("Dark!")]
    public void Should_Have_Error_When_Theme_Invalid(string theme)
    {
        var dto = new UpdateUserProfileDto { ThemePreference = theme };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.ThemePreference);
    }

    /// <summary>
    /// Допустимые значения темы ("light", "dark") не должны вызывать ошибок.
    /// </summary>
    [Theory]
    [InlineData("light")]
    [InlineData("dark")]
    [InlineData("Light")]
    [InlineData("DARK")]
    public void Should_Not_Have_Error_When_Theme_Valid(string theme)
    {
        var dto = new UpdateUserProfileDto { ThemePreference = theme };
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveValidationErrorFor(x => x.ThemePreference);
    }

    // Время напоминания
    /// <summary>
    /// Время напоминания должно быть в формате HH:mm.
    /// </summary>
    [Theory]
    [InlineData("25:00")]
    [InlineData("12:60")]
    [InlineData("abc")]
    public void Should_Have_Error_When_ReminderTime_Invalid(string time)
    {
        var dto = new UpdateUserProfileDto { HabitReminderTime = time };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.HabitReminderTime);
    }

    /// <summary>
    /// Корректное время напоминания (например, "08:00") не должно вызывать ошибок.
    /// </summary>
    [Fact]
    public void Should_Not_Have_Error_When_ReminderTime_Valid()
    {
        var dto = new UpdateUserProfileDto { HabitReminderTime = "08:00" };
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveValidationErrorFor(x => x.HabitReminderTime);
    }

    // Полностью валидный объект
    /// <summary>
    /// Корректно заполненный DTO не должен вызывать ошибок валидации.
    /// </summary>
    [Fact]
    public void Should_Not_Have_Error_When_Valid()
    {
        var dto = new UpdateUserProfileDto
        {
            Name = "User",
            City = "Москва",
            ThemePreference = "dark",
            HabitReminderTime = "08:00",
            HabitReminderEnabled = true
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
        var dto = new UpdateUserProfileDto();
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
