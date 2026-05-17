using HabitApi.Services.Interfaces;

namespace HabitApi.Tests.Integration.Infrastructure;

internal sealed class TestEmailService : IEmailService
{
    public List<(string ToEmail, string ConfirmationLink)> SentMessages { get; } = new();

    public Task SendConfirmationEmailAsync(string toEmail, string confirmationLink)
    {
        SentMessages.Add((toEmail, confirmationLink));
        return Task.CompletedTask;
    }
}
