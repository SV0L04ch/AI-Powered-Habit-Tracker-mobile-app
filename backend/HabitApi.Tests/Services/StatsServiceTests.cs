using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using HabitApi.Data;
using HabitApi.Models.Domain;
using HabitApi.Models.DTO;
using HabitApi.Services;
using HabitApi.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace HabitApi.Tests.Services;

/// <summary>
/// Модульные тесты для <see cref="StatsService"/>.
/// Проверяет формирование ежедневной персональной сводки и еженедельной городской статистики.
/// </summary>
public class StatsServiceTests
{
    /// <summary>
    /// Создаёт контекст <see cref="AppDbContext"/> с InMemory базой данных для изоляции тестов.
    /// </summary>
    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    /// <summary>
    /// Проверяет, что ежедневная сводка правильно подсчитывает количество выполненных, частично выполненных и пропущенных привычек.
    /// </summary>
    [Fact]
    public async Task GetDailySummaryAsync_ReturnsCorrectCounts()
    {
        // Arrange
        var context = CreateContext();
        var userId = Guid.NewGuid();
        var date = new DateOnly(2025, 1, 1);

        var user = new ApplicationUser { Id = userId, Email = "test@test.com", City = "Moscow" };
        context.Users.Add(user);

        var habit1 = new Habit { Id = Guid.NewGuid(), UserId = userId, IsActive = true, Name = "Бег", TriggerType = TriggerType.TimeOfDay, TriggerValue = "08:00", IsPositive = true };
        var habit2 = new Habit { Id = Guid.NewGuid(), UserId = userId, IsActive = true, Name = "Чтение", TriggerType = TriggerType.CountPerDay, TriggerValue = "1", IsPositive = true };
        context.Habits.AddRange(habit1, habit2);

        context.HabitEntries.AddRange(
            new HabitEntry { HabitId = habit1.Id, Date = date, Status = HabitEntryStatus.Completed },
            new HabitEntry { HabitId = habit2.Id, Date = date, Status = HabitEntryStatus.Partial, PartialValue = 1 },
            new HabitEntry { HabitId = habit2.Id, Date = date.AddDays(1), Status = HabitEntryStatus.Completed } // другой день, не должен учитываться
        );
        await context.SaveChangesAsync();

        var weatherServiceMock = new Mock<IWeatherService>();
        weatherServiceMock.Setup(w => w.GetWeatherAsync(user.City, date, It.IsAny<CancellationToken>()))
                          .ReturnsAsync(new WeatherSnapshotDto { City = user.City, Date = date, Condition = "Clear" });

        var aiServiceMock = new Mock<IAiInsightsService>();
        aiServiceMock.Setup(a => a.BuildDailyInsightAsync(It.IsAny<DailySummaryDto>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(new AiInsightResultDto { Message = "Хороший день!" });

        var service = new StatsService(context, weatherServiceMock.Object, aiServiceMock.Object, Mock.Of<ILogger<StatsService>>());

        // Act
        var summary = await service.GetDailySummaryAsync(userId, date, CancellationToken.None);

        // Assert
        Assert.NotNull(summary);
        Assert.Equal(date, summary.Date);
        Assert.Equal(1, summary.HabitsCompleted);
        Assert.Equal(1, summary.HabitsPartiallyCompleted);
        Assert.Equal(0, summary.HabitsSkipped);
        Assert.Equal("Clear", summary.Weather?.Condition);
        Assert.Equal("Хороший день!", summary.AiInsight);
        Assert.False(summary.IsAiInsightFallback);
    }

    [Fact]
    public async Task GetDailySummaryAsync_AiServiceThrows_ReturnsFallbackInsight()
    {
        var context = CreateContext();
        var userId = Guid.NewGuid();
        var date = new DateOnly(2025, 1, 1);
        var user = new ApplicationUser { Id = userId, Email = "test@test.com", City = "Moscow" };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var weatherServiceMock = new Mock<IWeatherService>();
        weatherServiceMock.Setup(w => w.GetWeatherAsync(user.City, date, It.IsAny<CancellationToken>()))
                          .ReturnsAsync(new WeatherSnapshotDto { City = user.City, Date = date, Condition = "Clear" });

        var aiServiceMock = new Mock<IAiInsightsService>();
        aiServiceMock.Setup(a => a.BuildDailyInsightAsync(It.IsAny<DailySummaryDto>(), It.IsAny<CancellationToken>()))
                     .ThrowsAsync(new HttpRequestException("AI unavailable"));

        var service = new StatsService(context, weatherServiceMock.Object, aiServiceMock.Object, Mock.Of<ILogger<StatsService>>());

        var summary = await service.GetDailySummaryAsync(userId, date, CancellationToken.None);

        Assert.True(summary.IsAiInsightFallback);
        Assert.Equal("AI service is temporarily unavailable.", summary.AiInsightFallbackReason);
        Assert.NotEmpty(summary.AiInsight);
    }

    [Fact]
    public async Task GetDailySummaryAsync_WeatherServiceThrows_ReturnsFallbackWeather()
    {
        var context = CreateContext();
        var userId = Guid.NewGuid();
        var date = new DateOnly(2025, 1, 1);
        var user = new ApplicationUser { Id = userId, Email = "test@test.com", City = "Moscow" };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var weatherServiceMock = new Mock<IWeatherService>();
        weatherServiceMock.Setup(w => w.GetWeatherAsync(user.City, date, It.IsAny<CancellationToken>()))
                          .ThrowsAsync(new HttpRequestException("Weather unavailable"));

        var aiServiceMock = new Mock<IAiInsightsService>();
        aiServiceMock.Setup(a => a.BuildDailyInsightAsync(It.IsAny<DailySummaryDto>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(new AiInsightResultDto { Message = "Insight" });

        var service = new StatsService(context, weatherServiceMock.Object, aiServiceMock.Object, Mock.Of<ILogger<StatsService>>());

        var summary = await service.GetDailySummaryAsync(userId, date, CancellationToken.None);

        Assert.NotNull(summary.Weather);
        Assert.True(summary.Weather.IsFallback);
        Assert.Equal("Weather service is temporarily unavailable.", summary.Weather.FallbackReason);
    }

    /// <summary>
    /// Проверяет, что для несуществующего пользователя выбрасывается <see cref="KeyNotFoundException"/>.
    /// </summary>
    [Fact]
    public async Task GetDailySummaryAsync_UserNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var context = CreateContext();
        var service = new StatsService(context, Mock.Of<IWeatherService>(), Mock.Of<IAiInsightsService>(), Mock.Of<ILogger<StatsService>>());

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => service.GetDailySummaryAsync(Guid.NewGuid(), DateOnly.FromDateTime(DateTime.UtcNow), CancellationToken.None));
    }

    /// <summary>
    /// Проверяет, что городская сводка формируется корректно и содержит топ привычек.
    /// </summary>
    [Fact]
    public async Task GetWeeklyCitySummaryAsync_ReturnsTopHabits()
    {
        // Arrange
        var context = CreateContext();
        var city = "Moscow";

        var user1 = new ApplicationUser { Id = Guid.NewGuid(), Email = "u1@test.com", City = city };
        var user2 = new ApplicationUser { Id = Guid.NewGuid(), Email = "u2@test.com", City = city };
        context.Users.AddRange(user1, user2);

        var habit1 = new Habit { Id = Guid.NewGuid(), UserId = user1.Id, Name = "Бег", IsActive = true, TriggerType = TriggerType.TimeOfDay, TriggerValue = "08:00" };
        var habit2 = new Habit { Id = Guid.NewGuid(), UserId = user2.Id, Name = "Чтение", IsActive = true, TriggerType = TriggerType.CountPerDay, TriggerValue = "1" };
        context.Habits.AddRange(habit1, habit2);

        var weekStart = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-(int)DateTime.UtcNow.DayOfWeek + 1 - 7);
        context.HabitEntries.AddRange(
            new HabitEntry { HabitId = habit1.Id, Date = weekStart, Status = HabitEntryStatus.Completed },
            new HabitEntry { HabitId = habit2.Id, Date = weekStart.AddDays(1), Status = HabitEntryStatus.Completed }
        );
        await context.SaveChangesAsync();

        var service = new StatsService(context, Mock.Of<IWeatherService>(), Mock.Of<IAiInsightsService>(), Mock.Of<ILogger<StatsService>>());

        // Act
        var result = await service.GetWeeklyCitySummaryAsync(city, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(city, result.City);
        Assert.NotEmpty(result.PopularHabits);
        Assert.Equal(2, result.PopularHabits.Count);
    }
}
