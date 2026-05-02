namespace HabitApi.Models.DTO;

/// <summary>
/// DTO для ответа после успешной регистрации или входа.
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
    /// JWT access token (короткоживущий).
    /// </summary>
    public string AccessToken { get; set; } = string.Empty;

    // Рекомендуется добавить RefreshToken для обновления сессии без повторного ввода пароля.
    // public string RefreshToken { get; set; } = string.Empty;
}
