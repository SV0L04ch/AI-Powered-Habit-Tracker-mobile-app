using FluentValidation.TestHelper;
using HabitApi.Models.DTO;
using HabitApi.Validators;
using Xunit;

namespace HabitApi.Tests.Validators;

/// <summary>
/// Модульные тесты для <see cref="LoginRequestDtoValidator"/>.
/// Проверяет корректность введённого email и наличие пароля.
/// </summary>
public class LoginRequestDtoValidatorTests
{
    private readonly LoginRequestDtoValidator _validator = new();

    /// <summary>
    /// Пустой email должен вызывать ошибку валидации.
    /// </summary>
    [Fact]
    public void Should_Have_Error_When_Email_Is_Empty()
    {
        var dto = new LoginRequestDto { Email = "", Password = "123456" };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    /// <summary>
    /// Некорректный формат email должен вызывать ошибку валидации.
    /// </summary>
    [Fact]
    public void Should_Have_Error_When_Email_Is_Not_Valid_Format()
    {
        var dto = new LoginRequestDto { Email = "not-an-email", Password = "123456" };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    /// <summary>
    /// Email длиннее 256 символов должен вызывать ошибку валидации.
    /// </summary>
    [Fact]
    public void Should_Have_Error_When_Email_Is_Too_Long()
    {
        var longEmail = new string('a', 257) + "@example.com";
        var dto = new LoginRequestDto { Email = longEmail, Password = "123456" };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    /// <summary>
    /// Пустой пароль должен вызывать ошибку валидации.
    /// </summary>
    [Fact]
    public void Should_Have_Error_When_Password_Is_Empty()
    {
        var dto = new LoginRequestDto { Email = "user@example.com", Password = "" };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    /// <summary>
    /// Корректно заполненный DTO не должен вызывать ошибок валидации.
    /// </summary>
    [Fact]
    public void Should_Not_Have_Error_When_Valid()
    {
        var dto = new LoginRequestDto { Email = "test@test.com", Password = "Strong1!" };
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
