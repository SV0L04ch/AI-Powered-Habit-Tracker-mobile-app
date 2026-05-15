using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using HabitApi.Data;
using HabitApi.Models.Domain;
using HabitApi.Models.DTO;
using HabitApi.Services;

namespace HabitApi.Tests.Services;

public class HabitEntryServiceTests
{
    [Fact]
    public async Task CreateEntryAsync_ValidRequest_AddsAndReturnsDto()
    {
        var userId = Guid.NewGuid();
        var request = new CreateHabitEntryDto { HabitId = Guid.NewGuid(), IsCompleted = true, Notes = "done" };

        var mockSet = new Mock<DbSet<HabitEntry>>();
        var mockContext = new Mock<AppDbContext>();
        mockContext.Setup(c => c.HabitEntries).Returns(mockSet.Object);
        mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var service = new HabitEntryService(mockContext.Object);
        var result = await service.CreateEntryAsync(userId, request, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(request.HabitId, result.HabitId);
        mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}