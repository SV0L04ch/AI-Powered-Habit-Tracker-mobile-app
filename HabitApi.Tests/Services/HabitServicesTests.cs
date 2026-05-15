using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using HabitApi.Data;
using HabitApi.Models.Domain;
using HabitApi.Models.DTO;
using HabitApi.Services;

namespace HabitApi.Tests.Services;

public class HabitServiceTests
{
    private static Mock<DbSet<T>> CreateMockDbSet<T>(IQueryable<T> data) where T : class
    {
        var mockSet = new Mock<DbSet<T>>();
        mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(data.Provider);
        mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
        mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
        mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
        return mockSet;
    }

    [Fact]
    public async Task CreateHabitAsync_ValidRequest_ReturnsHabitDto()
    {
        var userId = Guid.NewGuid();
        var request = new CreateHabitDto { Name = "Чтение", IsPositive = true, TriggerType = TriggerType.CountPerDay, TriggerValue = "1", TargetDays = 30 };

        var mockSet = new Mock<DbSet<Habit>>();
        var mockContext = new Mock<AppDbContext>();
        mockContext.Setup(c => c.Habits).Returns(mockSet.Object);
        mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var service = new HabitService(mockContext.Object);
        var result = await service.CreateHabitAsync(userId, request, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("Чтение", result.Name);
        mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetUserHabitsAsync_ReturnsOnlyActiveUserHabits()
    {
        var userId = Guid.NewGuid();
        var habits = new List<Habit>
        {
            new() { Id = Guid.NewGuid(), UserId = userId, Name = "Бег", IsActive = true },
            new() { Id = Guid.NewGuid(), UserId = userId, Name = "Курение", IsActive = false },
            new() { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), Name = "Чужое", IsActive = true }
        }.AsQueryable();

        var mockSet = CreateMockDbSet(habits);
        var mockContext = new Mock<AppDbContext>();
        mockContext.Setup(c => c.Habits).Returns(mockSet.Object);

        var service = new HabitService(mockContext.Object);
        var result = await service.GetUserHabitsAsync(userId, CancellationToken.None);

        Assert.Single(result);
        Assert.Equal("Бег", result.First().Name);
    }

    [Fact]
    public async Task DeleteHabitAsync_ExistingHabit_ReturnsTrueAndDeactivates()
    {
        var userId = Guid.NewGuid();
        var habitId = Guid.NewGuid();
        var habit = new Habit { Id = habitId, UserId = userId, IsActive = true };

        var mockSet = new Mock<DbSet<Habit>>();
        var mockContext = new Mock<AppDbContext>();
        mockContext.Setup(c => c.Habits).Returns(mockSet.Object);
        mockContext.Setup(c => c.Habits.FindAsync(habitId, It.IsAny<CancellationToken>())).ReturnsAsync(habit);
        mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var service = new HabitService(mockContext.Object);
        var result = await service.DeleteHabitAsync(userId, habitId, CancellationToken.None);

        Assert.True(result);
        Assert.False(habit.IsActive);
    }

    [Fact]
    public async Task DeleteHabitAsync_NotFound_ReturnsFalse()
    {
        var userId = Guid.NewGuid();
        var habitId = Guid.NewGuid();

        var mockSet = new Mock<DbSet<Habit>>();
        var mockContext = new Mock<AppDbContext>();
        mockContext.Setup(c => c.Habits).Returns(mockSet.Object);
        mockContext.Setup(c => c.Habits.FindAsync(habitId, It.IsAny<CancellationToken>())).ReturnsAsync((Habit?)null);

        var service = new HabitService(mockContext.Object);
        var result = await service.DeleteHabitAsync(userId, habitId, CancellationToken.None);

        Assert.False(result);
    }
}