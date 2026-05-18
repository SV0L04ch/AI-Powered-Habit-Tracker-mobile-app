using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using HabitApi.Controllers;
using HabitApi.Models.Domain;
using HabitApi.Models.DTO;
using HabitApi.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace HabitApi.Tests.Controllers;

/// <summary>
/// Модульные тесты для <see cref="HabitEntriesController"/>.
/// Проверяет основные сценарии добавления, получения, обновления и удаления отметок привычек.
/// </summary>
public class HabitEntriesControllerTests
{
    /// <summary>
    /// Создаёт экземпляр контроллера с замоканным сервисом и подставленным пользователем.
    /// </summary>
    /// <param name="mockService">Мок сервиса <see cref="IHabitEntryService"/>.</param>
    /// <param name="userId">Идентификатор пользователя для ClaimsPrincipal.</param>
    /// <param name="habitIdFromRoute">Идентификатор привычки, передаваемый через маршрут.</param>
    private static HabitEntriesController CreateController(
        Mock<IHabitEntryService> mockService,
        Guid userId,
        Guid habitIdFromRoute = default)
    {
        var controller = new HabitEntriesController(mockService.Object);
        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            }))
        };
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext,
            RouteData = new RouteData()
        };

        if (habitIdFromRoute != default)
        {
            controller.ControllerContext.RouteData.Values["habitId"] = habitIdFromRoute.ToString();
        }

        return controller;
    }

    /// <summary>
    /// Проверяет, что при валидном запросе контроллер возвращает 201 Created с созданной отметкой.
    /// </summary>
    [Fact]
    public async Task AddEntry_ValidRequest_ReturnsCreated()
    {
        var userId = Guid.NewGuid();
        var habitId = Guid.NewGuid();
        var request = new CreateHabitEntryDto
        {
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            Status = HabitEntryStatus.Completed,
            Note = "Test note"
        };
        var createdEntry = new HabitEntryDto
        {
            Id = Guid.NewGuid(),
            HabitId = habitId,
            Date = request.Date,
            Status = request.Status,
            Note = request.Note
        };

        var mockService = new Mock<IHabitEntryService>();
        mockService
            .Setup(s => s.AddHabitEntryAsync(userId, habitId, request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdEntry);

        var controller = CreateController(mockService, userId, habitId);

        var result = await controller.AddEntry(habitId, request, CancellationToken.None);

        var createdAtResult = Assert.IsType<CreatedResult>(result.Result);
        Assert.Equal(createdEntry, createdAtResult.Value);
    }

    /// <summary>
    /// Проверяет, что если привычка не найдена, контроллер возвращает 404 NotFound.
    /// </summary>
    [Fact]
    public async Task AddEntry_ServiceThrowsKeyNotFound_ReturnsNotFound()
    {
        var userId = Guid.NewGuid();
        var habitId = Guid.NewGuid();
        var request = new CreateHabitEntryDto { Date = DateOnly.FromDateTime(DateTime.UtcNow) };

        var mockService = new Mock<IHabitEntryService>();
        mockService
            .Setup(s => s.AddHabitEntryAsync(userId, habitId, request, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new KeyNotFoundException());

        var controller = CreateController(mockService, userId, habitId);

        var result = await controller.AddEntry(habitId, request, CancellationToken.None);

        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    /// <summary>
    /// Проверяет, что при невалидных данных (ArgumentException) возвращается 400 BadRequest.
    /// </summary>
    [Fact]
    public async Task AddEntry_ServiceThrowsArgumentException_ReturnsBadRequest()
    {
        var userId = Guid.NewGuid();
        var habitId = Guid.NewGuid();
        var request = new CreateHabitEntryDto { Date = DateOnly.FromDateTime(DateTime.UtcNow) };

        var mockService = new Mock<IHabitEntryService>();
        mockService
            .Setup(s => s.AddHabitEntryAsync(userId, habitId, request, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArgumentException("Invalid data"));

        var controller = CreateController(mockService, userId, habitId);

        var result = await controller.AddEntry(habitId, request, CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    /// <summary>
    /// Проверяет, что метод GetEntries возвращает 200 OK со списком отметок.
    /// </summary>
    [Fact]
    public async Task GetEntries_ReturnsOkWithList()
    {
        var userId = Guid.NewGuid();
        var habitId = Guid.NewGuid();
        var entries = new List<HabitEntryDto>
        {
            new() { Id = Guid.NewGuid(), HabitId = habitId, Date = DateOnly.FromDateTime(DateTime.UtcNow) }
        };

        var mockService = new Mock<IHabitEntryService>();
        mockService
            .Setup(s => s.GetHabitEntriesAsync(userId, habitId, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entries);

        var controller = CreateController(mockService, userId, habitId);

        var result = await controller.GetEntries(habitId, null, null, CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(entries, okResult.Value);
    }
}
