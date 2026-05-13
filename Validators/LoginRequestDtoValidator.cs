using FluentValidation;
using HabitApi.Models.DTO;

namespace HabitApi.Validators;

/// <summary>
/// Валидатор запроса на вход пользователя.
/// Проверяет корректность email и наличие пароля.
/// </summary>
public sealed class LoginRequestDtoValidator : AbstractValidator<LoginRequestDto>
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
