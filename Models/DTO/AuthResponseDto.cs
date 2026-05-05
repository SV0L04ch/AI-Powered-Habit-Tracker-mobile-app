namespace HabitApi.Models.DTO;

/// <summary>
/// DTO для ответа после успешного входа.
/// </summary>
public sealed class AuthResponseDto
{
    /// <summary>
    /// Идентификатор пользователя.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Электронная почта пользователя.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// JWT access token (может быть null, если токен не выдаётся, например, при неподтверждённом email).
    /// </summary>
    public string? AccessToken { get; set; }
}
