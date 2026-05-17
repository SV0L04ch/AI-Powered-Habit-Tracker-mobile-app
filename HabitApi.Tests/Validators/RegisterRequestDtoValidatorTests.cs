using Xunit;
using FluentValidation.TestHelper;
using HabitApi.Models.DTO;
using HabitApi.Validators;

namespace HabitApi.Tests.Validators;

public class RegisterRequestDtoValidatorTests
{
    private readonly RegisterRequestDtoValidator _validator = new();

    [Fact]
    public void Should_Have_Error_When_Email_Invalid()
    {
        var dto = new RegisterRequestDto { Email = "not-email", Password = "Strong1!", UserName = "user" };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Should_Have_Error_When_Password_Too_Short()
    {
        var dto = new RegisterRequestDto { Email = "test@test.com", Password = "123", UserName = "user" };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Valid()
    {
        var dto = new RegisterRequestDto
        {
            Email = "test@test.com",
            Password = "Strong1!",
            UserName = "testuser",
            City = "Москва"
        };
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }
}