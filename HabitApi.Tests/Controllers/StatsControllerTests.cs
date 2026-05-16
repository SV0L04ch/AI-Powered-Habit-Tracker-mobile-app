using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using HabitApi.Controllers;
using HabitApi.Models.DTO;
using HabitApi.Services.Interfaces;

namespace HabitApi.Tests.Controllers;

public class StatsControllerTests
{
    private static StatsController CreateController(Mock<IStatsService> mockService, Guid userId)
    {
        var controller = new StatsController(mockService.Object);
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
    public async Task GetDailySummary_ReturnsOkWithSummary()
    {
        var userId = Guid.NewGuid();
        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        var expectedSummary = new DailySummaryDto
        {
            Date = date,
            CompletedCount = 3,
            TotalHabits = 5,
            Habits = new List<HabitDto>()
        };

        var mockService = new Mock<IStatsService>();
        mockService.Setup(s => s.GetDailySummaryAsync(userId, date, It.IsAny<CancellationToken>()))
                   .ReturnsAsync(expectedSummary);

        var controller = CreateController(mockService, userId);
        var result = await controller.GetDailySummary(date, CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var summary = Assert.IsType<DailySummaryDto>(okResult.Value);
        Assert.Equal(3, summary.CompletedCount);
        Assert.Equal(5, summary.TotalHabits);
    }

    [Fact]
    public async Task GetWeeklySummary_ReturnsOkWithSummary()
    {
        var userId = Guid.NewGuid();
        var weekStart = DateOnly.FromDateTime(DateTime.UtcNow);
        var expected = new WeeklySummaryDto
        {
            StartDate = weekStart,
            EndDate = weekStart.AddDays(6),
            CompletedCount = 12,
            TotalHabits = 30
        };

        var mockService = new Mock<IStatsService>();
        mockService.Setup(s => s.GetWeeklySummaryAsync(userId, weekStart, It.IsAny<CancellationToken>()))
                   .ReturnsAsync(expected);

        var controller = CreateController(mockService, userId);
        var result = await controller.GetWeeklySummary(weekStart, CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var summary = Assert.IsType<WeeklySummaryDto>(okResult.Value);
        Assert.Equal(12, summary.CompletedCount);
    }

    [Fact]
    public async Task GetMonthlySummary_ReturnsOkWithSummary()
    {
        var userId = Guid.NewGuid();
        var month = new DateOnly(2025, 1, 1);
        var expected = new MonthlySummaryDto
        {
            Month = month,
            CompletedCount = 50,
            TotalHabits = 100
        };

        var mockService = new Mock<IStatsService>();
        mockService.Setup(s => s.GetMonthlySummaryAsync(userId, month, It.IsAny<CancellationToken>()))
                   .ReturnsAsync(expected);

        var controller = CreateController(mockService, userId);
        var result = await controller.GetMonthlySummary(month, CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var summary = Assert.IsType<MonthlySummaryDto>(okResult.Value);
        Assert.Equal(50, summary.CompletedCount);
    }
}