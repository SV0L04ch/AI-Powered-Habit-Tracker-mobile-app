using Xunit;
using Microsoft.EntityFrameworkCore;
using HabitApi.Data;
using HabitApi.Models.Domain;
using HabitApi.Models.DTO;
using HabitApi.Services;

namespace HabitApi.Tests.Services;

/// <summary>
/// Модульные тесты для <see cref="HabitService"/>.
/// Проверяет создание, получение, обновление и мягкое удаление привычек.
/// </summary>
public class HabitServiceTests
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
    /// Проверяет, что при корректных данных создаётся новая привычка и возвращается её DTO.
    /// </summary>
    [Fact]
    public async Task CreateHabitAsync_ValidRequest_ReturnsHabitDto()
    {
        // Arrange
        var context = CreateContext();
        var userId = Guid.NewGuid();
        // Пользователь должен существовать для внешнего ключа
        context.Users.Add(new ApplicationUser { Id = userId, Email = "test@test.com", City = "Moscow" });
        await context.SaveChangesAsync();

        var request = new CreateHabitDto
        {
            Name = "Чтение",
            IsPositive = true,
            TriggerType = TriggerType.CountPerDay,
            TriggerValue = "1",
            TargetDays = 30
        };

        var service = new HabitService(context);

        // Act
        var result = await service.CreateHabitAsync(userId, request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Чтение", result.Name);
        Assert.True(result.IsActive);
        Assert.NotEqual(Guid.Empty, result.Id);
    }

    /// <summary>
    /// Проверяет, что создание привычки с невалидным триггером выбрасывает <see cref="ArgumentException"/>.
    /// </summary>
    [Fact]
    public async Task CreateHabitAsync_InvalidTrigger_ThrowsArgumentException()
    {
        // Arrange
        var context = CreateContext();
        var userId = Guid.NewGuid();
        context.Users.Add(new ApplicationUser { Id = userId, Email = "test@test.com", City = "Moscow" });
        await context.SaveChangesAsync();

        var request = new CreateHabitDto
        {
            Name = "Плохая привычка",
            IsPositive = true,
            TriggerType = TriggerType.TimeOfDay,
            TriggerValue = "не_время",   // некорректное значение
            TargetDays = 30
        };

        var service = new HabitService(context);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => service.CreateHabitAsync(userId, request, CancellationToken.None));
    }

    /// <summary>
    /// Проверяет, что метод GetUserHabitsAsync возвращает только активные привычки пользователя.
    /// </summary>
    [Fact]
    public async Task GetUserHabitsAsync_ReturnsOnlyActiveUserHabits()
    {
        // Arrange
        var context = CreateContext();
        var userId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        context.Users.AddRange(
            new ApplicationUser { Id = userId, Email = "user@test.com", City = "Moscow" },
            new ApplicationUser { Id = otherUserId, Email = "other@test.com", City = "Moscow" }
        );
        context.Habits.AddRange(
            new Habit { UserId = userId, Name = "Бег", IsActive = true },
            new Habit { UserId = userId, Name = "Курение", IsActive = false },  // неактивная
            new Habit { UserId = otherUserId, Name = "Чужое", IsActive = true }
        );
        await context.SaveChangesAsync();

        var service = new HabitService(context);

        // Act
        var result = await service.GetUserHabitsAsync(userId, CancellationToken.None);

        // Assert
        Assert.Single(result);
        Assert.Equal("Бег", result.First().Name);
    }

    /// <summary>
    /// Проверяет успешное обновление привычки.
    /// </summary>
    [Fact]
    public async Task UpdateHabitAsync_ValidRequest_UpdatesAndReturnsDto()
    {
        // Arrange
        var context = CreateContext();
        var userId = Guid.NewGuid();
        context.Users.Add(new ApplicationUser { Id = userId, Email = "test@test.com", City = "Moscow" });
        var habit = new Habit
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Name = "Старое имя",
            IsPositive = true,
            TriggerType = TriggerType.CountPerDay,
            TriggerValue = "1",
            IsActive = true
        };
        context.Habits.Add(habit);
        await context.SaveChangesAsync();

        var updateRequest = new UpdateHabitDto { Name = "Новое имя" };
        var service = new HabitService(context);

        // Act
        var result = await service.UpdateHabitAsync(userId, habit.Id, updateRequest, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Новое имя", result.Name);
    }

    /// <summary>
    /// Проверяет, что при обновлении несуществующей привычки возвращается null.
    /// </summary>
    [Fact]
    public async Task UpdateHabitAsync_NotFound_ReturnsNull()
    {
        // Arrange
        var context = CreateContext();
        var userId = Guid.NewGuid();
        var service = new HabitService(context);
        var updateRequest = new UpdateHabitDto { Name = "Неважно" };

        // Act
        var result = await service.UpdateHabitAsync(userId, Guid.NewGuid(), updateRequest, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Проверяет успешное мягкое удаление привычки (IsActive становится false).
    /// </summary>
    [Fact]
    public async Task DeleteHabitAsync_ExistingHabit_ReturnsTrueAndDeactivates()
    {
        // Arrange
        var context = CreateContext();
        var userId = Guid.NewGuid();
        context.Users.Add(new ApplicationUser { Id = userId, Email = "test@test.com", City = "Moscow" });
        var habit = new Habit { Id = Guid.NewGuid(), UserId = userId, IsActive = true };
        context.Habits.Add(habit);
        await context.SaveChangesAsync();

        var service = new HabitService(context);

        // Act
        var result = await service.DeleteHabitAsync(userId, habit.Id, CancellationToken.None);

        // Assert
        Assert.True(result);
        Assert.False(habit.IsActive);
    }

    /// <summary>
    /// Проверяет, что при попытке удалить несуществующую привычку возвращается false.
    /// </summary>
    [Fact]
    public async Task DeleteHabitAsync_NotFound_ReturnsFalse()
    {
        // Arrange
        var context = CreateContext();
        var userId = Guid.NewGuid();
        var service = new HabitService(context);

        // Act
        var result = await service.DeleteHabitAsync(userId, Guid.NewGuid(), CancellationToken.None);

        // Assert
        Assert.False(result);
    }
}
