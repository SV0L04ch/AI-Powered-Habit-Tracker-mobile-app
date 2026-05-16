using Xunit;
using FluentValidation.TestHelper;
using HabitApi.Models.DTO;
using HabitApi.Validators;

namespace HabitApi.Tests.Validators;

public class CreateHabitEntryDtoValidatorTests
{
    private readonly CreateHabitEntryDtoValidator _validator = new();

    [Fact]
    public void Should_Have_Error_When_HabitId_Empty()
    {
        var dto = new CreateHabitEntryDto { HabitId = Guid.Empty, IsCompleted = true };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.HabitId);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Valid()
    {
        var dto = new CreateHabitEntryDto { HabitId = Guid.NewGuid(), IsCompleted = false, Notes = "some" };
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }
}