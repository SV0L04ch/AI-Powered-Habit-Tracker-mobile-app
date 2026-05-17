using System.Net;
using System.Net.Mail;
using HabitApi.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace HabitApi.Services;

/// <summary>
/// Сервис для отправки электронных писем через SMTP (MailHog).
/// </summary>
public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;

    public EmailService(ILogger<EmailService> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task SendConfirmationEmailAsync(string toEmail, string confirmationLink)
    {
        _logger.LogInformation("Trying to send email to {ToEmail} via mailhog", toEmail);
        try
        {
            using var client = new SmtpClient("mailhog", 1025);
            client.EnableSsl = false;
            client.UseDefaultCredentials = true;

            var mail = new MailMessage
            {
                From = new MailAddress("noreply@habittracker.local", "Habit Tracker"),
                Subject = "Подтверждение регистрации",
                Body = $"<h2>Добро пожаловать в Habit Tracker!</h2><p>Перейдите по ссылке для подтверждения:</p><a href='{confirmationLink}'>Подтвердить регистрацию</a>",
                IsBodyHtml = true
            };
            mail.To.Add(toEmail);

            await client.SendMailAsync(mail);
            _logger.LogInformation("Email sent successfully to {ToEmail}", toEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {ToEmail}", toEmail);
            throw;
        }
    }
}
