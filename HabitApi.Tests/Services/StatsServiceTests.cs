using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using HabitApi.Data;
using HabitApi.Models.Domain;
using HabitApi.Services;

namespace HabitApi.Tests.Services;

public class StatsServiceTests
{
    [Fact]
    public async Task GetDailySummaryAsync_ReturnsCorrectCounts()
    {
        var userId = Guid.NewGuid();
        var date = new DateOnly(2025, 1, 1);
        var entries = new List<HabitEntry>
        {
            new() { UserId = userId, Date = date, IsCompleted = true },
            new() { UserId = userId, Date = date, IsCompleted = false },
            new() { UserId = userId, Date = date.AddDays(1), IsCompleted = true }
        }.AsQueryable();

        var mockSet = new Mock<DbSet<HabitEntry>>();
        mockSet.As<IQueryable<HabitEntry>>().Setup(m => m.Provider).Returns(entries.Provider);
        mockSet.As<IQueryable<HabitEntry>>().Setup(m => m.Expression).Returns(entries.Expression);
        mockSet.As<IQueryable<HabitEntry>>().Setup(m => m.ElementType).Returns(entries.ElementType);
        mockSet.As<IQueryable<HabitEntry>>().Setup(m => m.GetEnumerator()).Returns(entries.GetEnumerator());

        var mockContext = new Mock<AppDbContext>();
        mockContext.Setup(c => c.HabitEntries).Returns(mockSet.Object);

        var service = new StatsService(mockContext.Object);
        var summary = await service.GetDailySummaryAsync(userId, date);

        Assert.Equal(2, summary.TotalHabits);
        Assert.Equal(1, summary.CompletedCount);
    }
}