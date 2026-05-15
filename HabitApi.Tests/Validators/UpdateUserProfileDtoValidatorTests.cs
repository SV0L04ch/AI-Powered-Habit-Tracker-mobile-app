using Xunit;
using FluentValidation.TestHelper;
using HabitApi.Models.DTO;
using HabitApi.Validators;

namespace HabitApi.Tests.Validators;

public class UpdateUserProfileDtoValidatorTests
{
    private readonly UpdateUserProfileDtoValidator _validator = new();

    [Fact]
    public void Should_Have_Error_When_City_Too_Long()
    {
        var dto = new UpdateUserProfileDto { City = new string('A', 101) };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.City);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Valid()
    {
        var dto = new UpdateUserProfileDto { City = "Москва" };
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }
}