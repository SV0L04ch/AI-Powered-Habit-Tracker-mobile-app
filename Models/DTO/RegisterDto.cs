using System.ComponentModel.DataAnnotations;

namespace HabitApi.Models.DTO;

/// <summary>
/// DTO для запроса на регистрацию нового пользователя.
/// </summary>
public sealed class RegisterRequestDto
{
    /// <summary>
    /// Электронная почта пользователя (используется как логин).
    /// </summary>
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Пароль пользователя. Минимальная длина 6 символов.
    /// </summary>
    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Город пользователя (для погодной аналитики и городской сводки).
    /// </summary>
    [Required]
    public string City { get; set; } = string.Empty;
}

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
