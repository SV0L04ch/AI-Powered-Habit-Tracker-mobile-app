using System.Net;
using System.Net.Mail;
using HabitApi.Services.Interfaces;

namespace HabitApi.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendConfirmationEmailAsync(string toEmail, string confirmationLink)
    {
        _logger.LogInformation("Starting email send to {ToEmail}", toEmail);
        try
        {
            var smtpServer = _configuration["EmailSettings:SmtpServer"];
            var smtpPort = int.TryParse(_configuration["EmailSettings:SmtpPort"], out var port) ? port : 1025;
            var login = _configuration["EmailSettings:Login"];
            var password = _configuration["EmailSettings:Password"];
            var fromEmail = _configuration["EmailSettings:SenderEmail"] ?? "noreply@habittracker.local";
            var fromName = _configuration["EmailSettings:SenderName"] ?? "Habit Tracker";
            var enableSsl = bool.TryParse(_configuration["EmailSettings:EnableSsl"], out var ssl) && ssl;

            _logger.LogDebug("SMTP settings: Server={Server}, Port={Port}, SSL={Ssl}, From={From}", smtpServer, smtpPort, enableSsl, fromEmail);

            using var client = new SmtpClient(smtpServer, smtpPort);
            client.EnableSsl = enableSsl;

            // Если указаны логин и пароль – используем аутентификацию (MailHog их не требует)
            if (!string.IsNullOrWhiteSpace(login) && !string.IsNullOrWhiteSpace(password))
            {
                client.Credentials = new NetworkCredential(login, password);
            }

            var mail = new MailMessage
            {
                From = new MailAddress(fromEmail, fromName),
                Subject = "Подтверждение регистрации",
                Body = $@"<h2>Добро пожаловать в Habit Tracker!</h2><p>Для подтверждения email перейдите по ссылке:</p><a href='{confirmationLink}'>Подтвердить регистрацию</a>",
                IsBodyHtml = true
            };
            mail.To.Add(toEmail);

            _logger.LogInformation("Sending email...");
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
