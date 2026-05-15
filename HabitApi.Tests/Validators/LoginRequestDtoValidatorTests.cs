using FluentValidation.TestHelper;
using HabitApi.Models.DTO;
using HabitApi.Validators;

namespace HabitApi.Tests.Validators;

public class LoginRequestDtoValidatorTests
{
    private readonly LoginRequestDtoValidator _validator = new();

    [Fact]
    public void Should_Have_Error_When_Email_Is_Empty()
    {
        var dto = new LoginRequestDto { Email = "", Password = "123456" };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Should_Have_Error_When_Email_Is_Not_Valid_Format()
    {
        var dto = new LoginRequestDto { Email = "not-an-email", Password = "123456" };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Should_Have_Error_When_Password_Is_Empty()
    {
        var dto = new LoginRequestDto { Email = "user@example.com", Password = "" };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void Should_Have_Error_When_Password_Is_Too_Short()
    {
        var dto = new LoginRequestDto { Email = "user@example.com", Password = "123" }; // меньше минимальной длины
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Valid()
    {
        var dto = new LoginRequestDto { Email = "test@test.com", Password = "Strong1!" };
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }
}