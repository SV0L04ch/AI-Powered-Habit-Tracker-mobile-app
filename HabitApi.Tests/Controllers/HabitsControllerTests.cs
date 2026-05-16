using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using HabitApi.Controllers;
using HabitApi.Models.DTO;
using HabitApi.Services.Interfaces;

namespace HabitApi.Tests.Controllers;

public class HabitsControllerTests
{
    private static HabitsController CreateController(Mock<IHabitService> mockService, Guid? userId = null)
    {
        var controller = new HabitsController(mockService.Object);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, (userId ?? Guid.NewGuid()).ToString())
                }))
            }
        };
        return controller;
    }

    [Fact]
    public async Task CreateHabit_ValidDto_ReturnsCreatedAtAction()
    {
        var request = new CreateHabitDto { Name = "Тест" };
        var dto = new HabitDto { Id = Guid.NewGuid(), Name = "Тест" };

        var mockService = new Mock<IHabitService>();
        mockService.Setup(s => s.CreateHabitAsync(It.IsAny<Guid>(), request, It.IsAny<CancellationToken>()))
                   .ReturnsAsync(dto);

        var controller = CreateController(mockService);
        var result = await controller.CreateHabit(request, CancellationToken.None);

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(dto, created.Value);
    }

    [Fact]
    public async Task GetHabitById_ExistingHabit_ReturnsOk()
    {
        var habitId = Guid.NewGuid();
        var dto = new HabitDto { Id = habitId, Name = "Тест" };

        var mockService = new Mock<IHabitService>();
        mockService.Setup(s => s.GetHabitByIdAsync(It.IsAny<Guid>(), habitId, It.IsAny<CancellationToken>()))
                   .ReturnsAsync(dto);

        var controller = CreateController(mockService);
        var result = await controller.GetHabitById(habitId, CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(dto, ok.Value);
    }

    [Fact]
    public async Task GetHabitById_NotFound_ReturnsNotFound()
    {
        var habitId = Guid.NewGuid();
        var mockService = new Mock<IHabitService>();
        mockService.Setup(s => s.GetHabitByIdAsync(It.IsAny<Guid>(), habitId, It.IsAny<CancellationToken>()))
                   .ReturnsAsync((HabitDto?)null);

        var controller = CreateController(mockService);
        var result = await controller.GetHabitById(habitId, CancellationToken.None);

        Assert.IsType<NotFoundObjectResult>(result.Result);
    }
}