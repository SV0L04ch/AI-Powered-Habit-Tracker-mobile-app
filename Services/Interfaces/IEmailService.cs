namespace HabitApi.Services.Interfaces;

public interface IEmailService
{
    Task SendConfirmationEmailAsync(string toEmail, string confirmationLink);
}
