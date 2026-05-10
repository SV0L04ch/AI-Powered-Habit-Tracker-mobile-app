namespace HabitApi.Models.DTO;

/// <summary>
/// DTO для запроса на регистрацию нового пользователя.
/// </summary>
public sealed class RegisterRequestDto
{
    /// <summary>
    /// Электронная почта пользователя (используется как логин).
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Пароль пользователя. Минимальная длина 6 символов.
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Город пользователя (для погодной аналитики и городской сводки).
    /// </summary>
    public string City { get; set; } = string.Empty;
}
