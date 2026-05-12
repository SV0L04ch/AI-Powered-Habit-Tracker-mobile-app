using FluentValidation;
using HabitApi.Models.DTO;

namespace HabitApi.Validators;

public class RegisterRequestDtoValidator : AbstractValidator<RegisterRequestDto>
{
    public RegisterRequestDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.")
            .MaximumLength(256).WithMessage("Email is too long (max 256 characters).");   // <-- добавлено

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters.")
            .Matches(@"[A-Za-z]").WithMessage("Password must contain at least one letter.")
            .Matches(@"[0-9]").WithMessage("Password must contain at least one digit.");

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("City is required.")
            .MaximumLength(100).WithMessage("City name is too long (max 100 characters).");
    }
}
