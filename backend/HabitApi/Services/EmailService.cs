using System.Net;
using System.Net.Mail;
using HabitApi.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace HabitApi.Services;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;
    private readonly IConfiguration _configuration;

    public EmailService(ILogger<EmailService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public async Task SendConfirmationEmailAsync(string toEmail, string confirmationLink)
    {
        if (IsGmailAddress(toEmail))
        {
            await SendViaGmailSmtp(toEmail, confirmationLink);
        }
        else
        {
            await SendViaMailHog(toEmail, confirmationLink);
        }
    }

    private async Task SendViaMailHog(string toEmail, string confirmationLink)
    {
        _logger.LogInformation("Sending email to {ToEmail} via MailHog", toEmail);
        try
        {
            using var client = new SmtpClient("mailhog", 1025);
            client.EnableSsl = false;
            client.UseDefaultCredentials = true;

            var mail = new MailMessage
            {
                From = new MailAddress("noreply@habittracker.local", "Habit Tracker"),
                Subject = "Confirm your registration",
                Body = $"<h2>Welcome to Habit Tracker!</h2><p>Click the link below to confirm your email:</p><a href='{confirmationLink}'>Confirm Registration</a>",
                IsBodyHtml = true
            };
            mail.To.Add(toEmail);

            await client.SendMailAsync(mail);
            _logger.LogInformation("Email sent via MailHog to {ToEmail}", toEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email via MailHog to {ToEmail}", toEmail);
            throw;
        }
    }

    private async Task SendViaGmailSmtp(string toEmail, string confirmationLink)
    {
        var senderEmail = Environment.GetEnvironmentVariable("GMAIL_SENDER_EMAIL");
        var appPassword = Environment.GetEnvironmentVariable("GMAIL_APP_PASSWORD");

        if (string.IsNullOrEmpty(senderEmail) || string.IsNullOrEmpty(appPassword))
        {
            _logger.LogWarning("Gmail credentials not configured, falling back to MailHog for {ToEmail}", toEmail);
            await SendViaMailHog(toEmail, confirmationLink);
            return;
        }

        _logger.LogInformation("Sending email to {ToEmail} via Gmail SMTP", toEmail);
        try
        {
            using var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(senderEmail, appPassword)
            };

            var mail = new MailMessage
            {
                From = new MailAddress(senderEmail, "Habit Tracker"),
                Subject = "Confirm your registration",
                Body = $"<h2>Welcome to Habit Tracker!</h2><p>Click the link below to confirm your email:</p><a href='{confirmationLink}'>Confirm Registration</a>",
                IsBodyHtml = true
            };
            mail.To.Add(toEmail);

            await client.SendMailAsync(mail);
            _logger.LogInformation("Email sent via Gmail to {ToEmail}", toEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email via Gmail to {ToEmail}", toEmail);
            throw;
        }
    }

    public static bool IsGmailAddress(string email)
    {
        return email.EndsWith("@gmail.com", StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsTestEmail(string email)
    {
        return !IsGmailAddress(email);
    }
}
