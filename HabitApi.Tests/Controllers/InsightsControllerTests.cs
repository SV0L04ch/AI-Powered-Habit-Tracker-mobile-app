using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using HabitApi.Controllers;
using HabitApi.Models.DTO;
using HabitApi.Services.Interfaces;

namespace HabitApi.Tests.Controllers;

public class InsightsControllerTests
{
    private static InsightsController CreateController(Mock<IHabitService> mockService, Guid userId)
    {
        var controller = new InsightsController(mockService.Object);
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
    public async Task GetBestHabit_ReturnsOkWithHabit()
    {
        var userId = Guid.NewGuid();
        var bestHabit = new HabitDto { Id = Guid.NewGuid(), Name = "Бег", CompletionRate = 0.95 };

        var mockService = new Mock<IHabitService>();
        mockService.Setup(s => s.GetBestHabitAsync(userId, It.IsAny<CancellationToken>()))
                   .ReturnsAsync(bestHabit);

        var controller = CreateController(mockService, userId);
        var result = await controller.GetBestHabit(CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(bestHabit, ok.Value);
    }

    [Fact]
    public async Task GetWorstHabit_ReturnsOkWithHabit()
    {
        var userId = Guid.NewGuid();
        var worstHabit = new HabitDto { Id = Guid.NewGuid(), Name = "Курение", CompletionRate = 0.1 };

        var mockService = new Mock<IHabitService>();
        mockService.Setup(s => s.GetWorstHabitAsync(userId, It.IsAny<CancellationToken>()))
                   .ReturnsAsync(worstHabit);

        var controller = CreateController(mockService, userId);
        var result = await controller.GetWorstHabit(CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(worstHabit, ok.Value);
    }
}