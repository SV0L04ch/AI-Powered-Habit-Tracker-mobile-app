using Xunit;
using Microsoft.EntityFrameworkCore;
using HabitApi.Data;
using HabitApi.Exceptions;
using HabitApi.Models.Domain;
using HabitApi.Models.DTO;
using HabitApi.Services;

namespace HabitApi.Tests.Services;

/// <summary>
/// Модульные тесты для <see cref="HabitEntryService"/>.
/// Проверяет добавление, получение, обновление и удаление отметок выполнения привычек.
/// </summary>
public class HabitEntryServiceTests
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
    /// Проверяет, что при корректных данных создаётся новая отметка выполнения привычки.
    /// </summary>
    [Fact]
    public async Task AddHabitEntryAsync_ValidRequest_ReturnsCreatedEntry()
    {
        // Arrange
        var context = CreateContext();
        var user = new ApplicationUser { Id = Guid.NewGuid(), Email = "test@test.com", City = "Moscow" };
        var habit = new Habit
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Name = "Чтение",
            IsPositive = true,
            TriggerType = TriggerType.CountPerDay,
            TriggerValue = "1",
            IsActive = true
        };
        context.Users.Add(user);
        context.Habits.Add(habit);
        await context.SaveChangesAsync();

        var service = new HabitEntryService(context);
        var request = new CreateHabitEntryDto
        {
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            Status = HabitEntryStatus.Completed,
            Note = "Прочитал 30 минут"
        };

        // Act
        var result = await service.AddHabitEntryAsync(user.Id, habit.Id, request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(habit.Id, result.HabitId);
        Assert.Equal(request.Status, result.Status);
        Assert.Equal(request.Note, result.Note);
    }

    /// <summary>
    /// Проверяет, что нельзя создать две отметки на одну дату для одной привычки.
    /// </summary>
    [Fact]
    public async Task AddHabitEntryAsync_DuplicateDate_ThrowsConflictException()
    {
        // Arrange
        var context = CreateContext();
        var user = new ApplicationUser { Id = Guid.NewGuid(), Email = "test@test.com", City = "Moscow" };
        var habit = new Habit
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Name = "Бег",
            IsPositive = true,
            TriggerType = TriggerType.TimeOfDay,
            TriggerValue = "08:00",
            IsActive = true
        };
        context.Users.Add(user);
        context.Habits.Add(habit);
        await context.SaveChangesAsync();

        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        var firstEntry = new HabitEntry { HabitId = habit.Id, Date = date, Status = HabitEntryStatus.Completed };
        context.HabitEntries.Add(firstEntry);
        await context.SaveChangesAsync();

        var service = new HabitEntryService(context);
        var request = new CreateHabitEntryDto { Date = date, Status = HabitEntryStatus.Completed };

        // Act & Assert
        await Assert.ThrowsAsync<ConflictException>(
            () => service.AddHabitEntryAsync(user.Id, habit.Id, request, CancellationToken.None));
    }

    /// <summary>
    /// Проверяет получение списка отметок за период.
    /// </summary>
    [Fact]
    public async Task GetHabitEntriesAsync_ReturnsEntriesForDateRange()
    {
        // Arrange
        var context = CreateContext();
        var user = new ApplicationUser { Id = Guid.NewGuid(), Email = "test@test.com", City = "Moscow" };
        var habit = new Habit
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Name = "Медитация",
            IsPositive = true,
            TriggerType = TriggerType.CountPerDay,
            TriggerValue = "2",
            IsActive = true
        };
        context.Users.Add(user);
        context.Habits.Add(habit);
        var entry1 = new HabitEntry { HabitId = habit.Id, Date = new DateOnly(2026, 5, 10), Status = HabitEntryStatus.Completed };
        var entry2 = new HabitEntry { HabitId = habit.Id, Date = new DateOnly(2026, 5, 12), Status = HabitEntryStatus.Partial, PartialValue = 1 };
        context.HabitEntries.AddRange(entry1, entry2);
        await context.SaveChangesAsync();

        var service = new HabitEntryService(context);

        // Act
        var result = await service.GetHabitEntriesAsync(user.Id, habit.Id, new DateOnly(2026, 5, 9), new DateOnly(2026, 5, 13), CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    /// <summary>
    /// Проверяет успешное обновление существующей отметки.
    /// </summary>
    [Fact]
    public async Task UpdateHabitEntryAsync_ValidRequest_UpdatesEntry()
    {
        // Arrange
        var context = CreateContext();
        var user = new ApplicationUser { Id = Guid.NewGuid(), Email = "test@test.com", City = "Moscow" };
        var habit = new Habit
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Name = "Йога",
            IsPositive = true,
            TriggerType = TriggerType.TimeOfDay,
            TriggerValue = "07:00",
            IsActive = true
        };
        context.Users.Add(user);
        context.Habits.Add(habit);
        var entry = new HabitEntry { HabitId = habit.Id, Date = DateOnly.FromDateTime(DateTime.UtcNow), Status = HabitEntryStatus.Completed };
        context.HabitEntries.Add(entry);
        await context.SaveChangesAsync();

        var service = new HabitEntryService(context);
        var updateRequest = new UpdateHabitEntryDto { Status = HabitEntryStatus.Skipped, Note = "Проспал" };

        // Act
        var result = await service.UpdateHabitEntryAsync(user.Id, habit.Id, entry.Id, updateRequest, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(HabitEntryStatus.Skipped, result.Status);
        Assert.Equal("Проспал", result.Note);
    }

    /// <summary>
    /// Проверяет успешное удаление существующей отметки.
    /// </summary>
    [Fact]
    public async Task DeleteHabitEntryAsync_ValidId_DeletesAndReturnsTrue()
    {
        // Arrange
        var context = CreateContext();
        var user = new ApplicationUser { Id = Guid.NewGuid(), Email = "test@test.com", City = "Moscow" };
        var habit = new Habit
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Name = "Бег",
            IsPositive = true,
            TriggerType = TriggerType.TimeOfDay,
            TriggerValue = "08:00",
            IsActive = true
        };
        context.Users.Add(user);
        context.Habits.Add(habit);
        var entry = new HabitEntry { HabitId = habit.Id, Date = DateOnly.FromDateTime(DateTime.UtcNow), Status = HabitEntryStatus.Completed };
        context.HabitEntries.Add(entry);
        await context.SaveChangesAsync();

        var service = new HabitEntryService(context);

        // Act
        var result = await service.DeleteHabitEntryAsync(user.Id, habit.Id, entry.Id, CancellationToken.None);

        // Assert
        Assert.True(result);
        Assert.Null(await context.HabitEntries.FindAsync(entry.Id));
    }
}
