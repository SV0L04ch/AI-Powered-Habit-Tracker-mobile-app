namespace HabitApi.Models.DTO;

/// <summary>
/// Ответ после успешной регистрации (без токена, только данные пользователя и сообщение).
/// </summary>
public sealed class RegistrationResponseDto
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
