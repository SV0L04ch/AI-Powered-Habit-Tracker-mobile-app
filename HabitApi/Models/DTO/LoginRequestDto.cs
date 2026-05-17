using System.ComponentModel.DataAnnotations;

namespace HabitApi.Models.DTO;

/// <summary>
/// DTO для запроса на вход пользователя.
/// </summary>
public sealed class LoginRequestDto
{
    /// <summary>
    /// Электронная почта пользователя.
    /// </summary>
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Пароль пользователя (в открытом виде).
    /// </summary>
    [Required]
    public string Password { get; set; } = string.Empty;
}
