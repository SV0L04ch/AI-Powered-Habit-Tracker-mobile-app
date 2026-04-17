using HabitApi.Models.DTO;

namespace HabitApi.Services.Interfaces;

/// <summary>
/// Сервис для аутентификации и управления пользователями.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Регистрация нового пользователя.
    /// </summary>
    /// <param name="request">Данные для регистрации (Email, Password, City).</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Ответ с токеном доступа и данными пользователя.</returns>
    /// <exception cref="InvalidOperationException">Если пользователь с таким email уже существует.</exception>
    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken);

    /// <summary>
    /// Вход пользователя в систему.
    /// </summary>
    /// <param name="request">Учётные данные (Email, Password).</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Ответ с токеном доступа, или null, если аутентификация не удалась.</returns>
    Task<AuthResponseDto?> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken);
}
