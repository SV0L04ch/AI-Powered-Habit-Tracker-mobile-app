namespace HabitApi.Models.DTO;

/// <summary>
/// Ответ после успешной регистрации.
/// Не содержит JWT-токен, так как пользователь должен сначала подтвердить email.
/// </summary>
public sealed class RegistrationResponseDto
{
    /// <summary>
    /// Идентификатор созданного пользователя.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Электронная почта пользователя.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Сообщение о необходимости подтверждения email.
    /// </summary>
    public string Message { get; set; } = string.Empty;
}
