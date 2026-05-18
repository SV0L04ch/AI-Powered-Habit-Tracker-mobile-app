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
/// Модульные тесты для <see cref="StatsController"/>.
/// Проверяет получение ежедневной персональной сводки и анонимной городской статистики.
/// </summary>
public class StatsControllerTests
{
    /// <summary>
    /// Создаёт контроллер с замоканным <see cref="IStatsService"/> и подставленным пользователем.
    /// </summary>
    private static StatsController CreateController(Mock<IStatsService> mockService, Guid? userId = null)
    {
        var controller = new StatsController(mockService.Object);
        if (userId.HasValue)
        {
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, userId.Value.ToString())
                    }))
                }
            };
        }
        return controller;
    }

    // DAILY SUMMARY
    /// <summary>
    /// Проверяет, что для авторизованного пользователя возвращается 200 OK с ежедневной сводкой.
    /// </summary>
    [Fact]
    public async Task GetDailySummary_ReturnsOkWithSummary()
    {
        var userId = Guid.NewGuid();
        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        var expectedSummary = new DailySummaryDto
        {
            Date = date,
            HabitsCompleted = 3,
            HabitsPartiallyCompleted = 1,
            HabitsSkipped = 1,
            Weather = null,
            AiInsight = "Хорошая работа!"
        };

        var mockService = new Mock<IStatsService>();
        mockService.Setup(s => s.GetDailySummaryAsync(userId, date, It.IsAny<CancellationToken>()))
                   .ReturnsAsync(expectedSummary);

        var controller = CreateController(mockService, userId);
        var result = await controller.GetDailySummary(date, CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var summary = Assert.IsType<DailySummaryDto>(okResult.Value);
        Assert.Equal(expectedSummary.HabitsCompleted, summary.HabitsCompleted);
        Assert.Equal(expectedSummary.AiInsight, summary.AiInsight);
    }

    /// <summary>
    /// Проверяет, что запрос будущей даты возвращает 400 BadRequest.
    /// </summary>
    [Fact]
    public async Task GetDailySummary_FutureDate_ReturnsBadRequest()
    {
        var userId = Guid.NewGuid();
        var futureDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2));

        var mockService = new Mock<IStatsService>();
        var controller = CreateController(mockService, userId);

        var result = await controller.GetDailySummary(futureDate, CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    /// <summary>
    /// Проверяет, что если сервис не находит пользователя, возвращается 404 NotFound.
    /// </summary>
    [Fact]
    public async Task GetDailySummary_UserNotFound_ReturnsNotFound()
    {
        var userId = Guid.NewGuid();
        var date = DateOnly.FromDateTime(DateTime.UtcNow);

        var mockService = new Mock<IStatsService>();
        mockService.Setup(s => s.GetDailySummaryAsync(userId, date, It.IsAny<CancellationToken>()))
                   .ThrowsAsync(new KeyNotFoundException("User not found."));

        var controller = CreateController(mockService, userId);
        var result = await controller.GetDailySummary(date, CancellationToken.None);

        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    // CITY SUMMARY
    /// <summary>
    /// Проверяет, что анонимный запрос городской сводки возвращает 200 OK с корректными данными.
    /// </summary>
    [Fact]
    public async Task GetCitySummary_ValidCity_ReturnsOkWithSummary()
    {
        var city = "Moscow";
        var expectedSummary = new CitySummaryDto
        {
            City = city,
            WeekStartDate = DateOnly.FromDateTime(DateTime.UtcNow),
            WeekEndDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(6),
            PopularHabits = new List<CityHabitStatDto>
            {
                new() { HabitName = "Бег", UserCount = 150, TotalUsers = 1000 }
            }
        };

        var mockService = new Mock<IStatsService>();
        mockService.Setup(s => s.GetWeeklyCitySummaryAsync(city, It.IsAny<CancellationToken>()))
                   .ReturnsAsync(expectedSummary);

        var controller = CreateController(mockService); // без userId – эндпоинт анонимный
        var result = await controller.GetCitySummary(city, CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var summary = Assert.IsType<CitySummaryDto>(okResult.Value);
        Assert.Equal(city, summary.City);
        Assert.NotEmpty(summary.PopularHabits);
    }

    /// <summary>
    /// Проверяет, что при пустом или не указанном городе возвращается 400 BadRequest.
    /// </summary>
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task GetCitySummary_MissingCity_ReturnsBadRequest(string city)
    {
        var mockService = new Mock<IStatsService>();
        var controller = CreateController(mockService);

        var result = await controller.GetCitySummary(city, CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }
}
