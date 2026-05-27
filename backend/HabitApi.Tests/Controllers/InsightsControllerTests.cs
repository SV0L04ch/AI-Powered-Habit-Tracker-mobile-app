using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using HabitApi.Controllers;
using HabitApi.Models.DTO;
using HabitApi.Services.Interfaces;
using Microsoft.AspNetCore.Http;

namespace HabitApi.Tests.Controllers;

/// <summary>
/// Модульные тесты для <see cref="InsightsController"/>.
/// Проверяет генерацию поддерживающих сообщений от ИИ для привычек.
/// </summary>
public class InsightsControllerTests
{
    /// <summary>
    /// Создаёт контроллер с замоканными <see cref="IHabitService"/> и <see cref="IAiInsightsService"/>,
    /// и подставленным пользователем.
    /// </summary>
    private static InsightsController CreateController(
        Mock<IHabitService> mockHabitService,
        Mock<IAiInsightsService> mockAiService,
        Guid userId)
    {
        var controller = new InsightsController(mockHabitService.Object, mockAiService.Object);
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

    /// <summary>
    /// Проверяет, что если привычка существует и ИИ возвращает сообщение, то контроллер отдаёт 200 OK с этим сообщением.
    /// </summary>
    [Fact]
    public async Task BuildSupportMessage_HabitFound_ReturnsOkWithMessage()
    {
        var userId = Guid.NewGuid();
        var habitId = Guid.NewGuid();
        var request = new HabitSupportRequestDto { Scenario = "lazy" };
        var habitDto = new HabitDto { Id = habitId, Name = "Утренняя зарядка" };
        var expectedMessage = "Просто начни с одного упражнения!";

        var mockHabit = new Mock<IHabitService>();
        mockHabit.Setup(s => s.GetHabitByIdAsync(userId, habitId, It.IsAny<CancellationToken>()))
                 .ReturnsAsync(habitDto);

        var mockAi = new Mock<IAiInsightsService>();
        mockAi.Setup(s => s.BuildHabitSupportMessageAsync(habitDto.Name, request.Scenario, It.IsAny<CancellationToken>()))
              .ReturnsAsync(new AiInsightResultDto { Message = expectedMessage });

        var controller = CreateController(mockHabit, mockAi, userId);
        var result = await controller.BuildSupportMessage(habitId, request, CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<HabitSupportResponseDto>(okResult.Value);
        Assert.Equal(habitId, response.HabitId);
        Assert.Equal(expectedMessage, response.Message);
        Assert.False(response.IsFallback);
    }

    /// <summary>
    /// Проверяет, что если привычка не найдена или не принадлежит пользователю, возвращается 404 NotFound.
    /// </summary>
    [Fact]
    public async Task BuildSupportMessage_HabitNotFound_ReturnsNotFound()
    {
        var userId = Guid.NewGuid();
        var habitId = Guid.NewGuid();
        var request = new HabitSupportRequestDto { Scenario = "lazy" };

        var mockHabit = new Mock<IHabitService>();
        mockHabit.Setup(s => s.GetHabitByIdAsync(userId, habitId, It.IsAny<CancellationToken>()))
                 .ReturnsAsync((HabitDto?)null);

        var mockAi = new Mock<IAiInsightsService>();
        var controller = CreateController(mockHabit, mockAi, userId);
        var result = await controller.BuildSupportMessage(habitId, request, CancellationToken.None);

        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    /// <summary>
    /// Проверяет, что если ИИ-сервис выбрасывает <see cref="ArgumentException"/>, контроллер возвращает 400 BadRequest.
    /// </summary>
    [Fact]
    public async Task BuildSupportMessage_InvalidScenario_ReturnsBadRequest()
    {
        var userId = Guid.NewGuid();
        var habitId = Guid.NewGuid();
        var request = new HabitSupportRequestDto { Scenario = "invalid_scenario" };
        var habitDto = new HabitDto { Id = habitId, Name = "Чтение" };

        var mockHabit = new Mock<IHabitService>();
        mockHabit.Setup(s => s.GetHabitByIdAsync(userId, habitId, It.IsAny<CancellationToken>()))
                 .ReturnsAsync(habitDto);

        var mockAi = new Mock<IAiInsightsService>();
        mockAi.Setup(s => s.BuildHabitSupportMessageAsync(habitDto.Name, request.Scenario, It.IsAny<CancellationToken>()))
              .ThrowsAsync(new ArgumentException("Unsupported scenario"));

        var controller = CreateController(mockHabit, mockAi, userId);
        var result = await controller.BuildSupportMessage(habitId, request, CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }
}
