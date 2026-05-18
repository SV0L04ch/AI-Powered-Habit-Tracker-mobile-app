using HabitApi.Models.DTO;
using HabitApi.Models.Domain;

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
    /// <returns>Ответ с данными пользователя и сообщением о необходимости подтверждения email.</returns>
    /// <exception cref="InvalidOperationException">Если пользователь с таким email уже существует.</exception>
    Task<RegistrationResponseDto> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken);

    /// <summary>
    /// Подтверждение email пользователя.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя.</param>
    /// <param name="token">Токен подтверждения.</param>
    /// <returns>Подтверждённый пользователь или null.</returns>
    Task<ApplicationUser?> ConfirmEmailAsync(Guid userId, string token);

    /// <summary>
    /// Вход пользователя в систему.
    /// </summary>
    /// <param name="request">Учётные данные (Email, Password).</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Ответ с токеном доступа, или null, если аутентификация не удалась.</returns>
    Task<AuthResponseDto?> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken);
}
