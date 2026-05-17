using Xunit;
using Moq;
using HabitApi.Services.Interfaces;

namespace HabitApi.Tests.Services;

public class EmailServiceTests
{
    [Fact]
    public async Task SendReminderEmailAsync_CallsSenderWithCorrectParams()
    {
        var mockSender = new Mock<IEmailSender>();
        var service = new EmailService(mockSender.Object);

        await service.SendReminderEmailAsync("user@example.com", "Reminder", "Don't forget!");

        mockSender.Verify(s => s.SendEmailAsync("user@example.com", "Reminder", "Don't forget!", It.IsAny<CancellationToken>()), Times.Once);
    }
}