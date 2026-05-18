using Xunit;
using FluentValidation.TestHelper;
using HabitApi.Models.DTO;
using HabitApi.Validators;

namespace HabitApi.Tests.Validators;

/// <summary>
/// Модульные тесты для <see cref="RegisterRequestDtoValidator"/>.
/// Проверяет правила валидации при регистрации нового пользователя.
/// </summary>
public class RegisterRequestDtoValidatorTests
{
    private readonly RegisterRequestDtoValidator _validator = new();

    // Email
    /// <summary>
    /// Пустой email должен вызывать ошибку валидации.
    /// </summary>
    [Fact]
    public void Should_Have_Error_When_Email_Empty()
    {
        var dto = new RegisterRequestDto { Email = "", Password = "Strong1!", City = "Moscow" };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    /// <summary>
    /// Некорректный формат email должен вызывать ошибку валидации.
    /// </summary>
    [Fact]
    public void Should_Have_Error_When_Email_Invalid()
    {
        var dto = new RegisterRequestDto { Email = "not-email", Password = "Strong1!", City = "Moscow" };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    /// <summary>
    /// Email длиннее 256 символов должен вызывать ошибку валидации.
    /// </summary>
    [Fact]
    public void Should_Have_Error_When_Email_Too_Long()
    {
        var longEmail = new string('a', 257) + "@example.com";
        var dto = new RegisterRequestDto { Email = longEmail, Password = "Strong1!", City = "Moscow" };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    // Пароль
    /// <summary>
    /// Пустой пароль должен вызывать ошибку валидации.
    /// </summary>
    [Fact]
    public void Should_Have_Error_When_Password_Empty()
    {
        var dto = new RegisterRequestDto { Email = "test@test.com", Password = "", City = "Moscow" };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    /// <summary>
    /// Пароль короче 6 символов должен вызывать ошибку валидации.
    /// </summary>
    [Fact]
    public void Should_Have_Error_When_Password_Too_Short()
    {
        var dto = new RegisterRequestDto { Email = "test@test.com", Password = "12345", City = "Moscow" };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    /// <summary>
    /// Пароль без цифр должен вызывать ошибку валидации.
    /// </summary>
    [Fact]
    public void Should_Have_Error_When_Password_Has_No_Digit()
    {
        var dto = new RegisterRequestDto { Email = "test@test.com", Password = "Strong!", City = "Moscow" };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    /// <summary>
    /// Пароль без букв должен вызывать ошибку валидации.
    /// </summary>
    [Fact]
    public void Should_Have_Error_When_Password_Has_No_Letter()
    {
        var dto = new RegisterRequestDto { Email = "test@test.com", Password = "123456", City = "Moscow" };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    // Город
    /// <summary>
    /// Пустой город должен вызывать ошибку валидации.
    /// </summary>
    [Fact]
    public void Should_Have_Error_When_City_Empty()
    {
        var dto = new RegisterRequestDto { Email = "test@test.com", Password = "Strong1!", City = "" };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.City);
    }

    /// <summary>
    /// Слишком длинное название города (более 100 символов) должно вызывать ошибку валидации.
    /// </summary>
    [Fact]
    public void Should_Have_Error_When_City_Too_Long()
    {
        var dto = new RegisterRequestDto { Email = "test@test.com", Password = "Strong1!", City = new string('A', 101) };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.City);
    }

    // Полностью валидный объект
    /// <summary>
    /// Корректно заполненный DTO не должен вызывать ошибок валидации.
    /// </summary>
    [Fact]
    public void Should_Not_Have_Error_When_Valid()
    {
        var dto = new RegisterRequestDto
        {
            Email = "test@test.com",
            Password = "Strong1!",
            City = "Москва"
        };
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
