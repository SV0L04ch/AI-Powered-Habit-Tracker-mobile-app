using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using HabitApi.Controllers;
using HabitApi.Models.DTO;
using HabitApi.Services.Interfaces;

namespace HabitApi.Tests.Controllers;

public class HabitEntriesControllerTests
{
    private static HabitEntriesController CreateController(Mock<IHabitEntryService> mockService, Guid userId)
    {
        var controller = new HabitEntriesController(mockService.Object);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString())
                }))
            }
        };
        return controller;
    }

    [Fact]
    public async Task CreateEntry_ValidRequest_ReturnsCreated()
    {
        var userId = Guid.NewGuid();
        var request = new CreateHabitEntryDto { HabitId = Guid.NewGuid(), IsCompleted = true };
        var createdEntry = new HabitEntryDto { Id = Guid.NewGuid(), HabitId = request.HabitId };

        var mockService = new Mock<IHabitEntryService>();
        mockService.Setup(s => s.CreateEntryAsync(userId, request, It.IsAny<CancellationToken>()))
                   .ReturnsAsync(createdEntry);

        var controller = CreateController(mockService, userId);
        var result = await controller.CreateEntry(request, CancellationToken.None);

        var createdAt = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(createdEntry, createdAt.Value);
    }

    [Fact]
    public async Task GetEntriesForHabit_ReturnsOkWithList()
    {
        var userId = Guid.NewGuid();
        var habitId = Guid.NewGuid();
        var entries = new List<HabitEntryDto> { new() { Id = Guid.NewGuid() } };

        var mockService = new Mock<IHabitEntryService>();
        mockService.Setup(s => s.GetEntriesForHabitAsync(userId, habitId, It.IsAny<CancellationToken>()))
                   .ReturnsAsync(entries);

        var controller = CreateController(mockService, userId);
        var result = await controller.GetEntriesForHabit(habitId, CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(entries, ok.Value);
    }
}