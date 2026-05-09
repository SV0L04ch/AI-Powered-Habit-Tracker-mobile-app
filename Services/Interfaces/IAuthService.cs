using HabitApi.Models.Domain; // ApplicationUser
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
    Task<RegistrationResponseDto> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken);

    /// <summary>
    /// Подтверждение email пользователя.
    /// </summary>
    Task<ApplicationUser?> ConfirmEmailAsync(Guid userId, string token);

    /// <summary>
    /// Вход пользователя в систему.
    /// </summary>
    Task<AuthResponseDto?> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken);
}
