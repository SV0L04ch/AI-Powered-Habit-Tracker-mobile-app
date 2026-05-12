using FluentValidation;
using HabitApi.Models.DTO;

namespace HabitApi.Validators;

public class LoginRequestDtoValidator : AbstractValidator<LoginRequestDto>
{
    public LoginRequestDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.")
            .MaximumLength(256).WithMessage("Email is too long (max 256 characters).");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.");
    }
}
