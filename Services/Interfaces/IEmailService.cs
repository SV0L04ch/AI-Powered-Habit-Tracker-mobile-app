namespace HabitApi.Services.Interfaces;

/// <summary>
/// Сервис для отправки электронных писем.
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Отправляет письмо со ссылкой для подтверждения email.
    /// </summary>
    /// <param name="toEmail">Адрес получателя.</param>
    /// <param name="confirmationLink">Ссылка для подтверждения регистрации.</param>
    /// <returns>Задача, представляющая асинхронную операцию отправки.</returns>
    Task SendConfirmationEmailAsync(string toEmail, string confirmationLink);
}
