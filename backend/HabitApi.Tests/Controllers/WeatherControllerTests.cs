using System.Net;
using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using HabitApi.Controllers;
using HabitApi.Models.DTO;
using HabitApi.Services.Interfaces;

namespace HabitApi.Tests.Controllers;

/// <summary>
/// Модульные тесты для <see cref="WeatherController"/>.
/// Проверяет получение погодных данных с учётом аутентификации.
/// </summary>
public class WeatherControllerTests
{
    /// <summary>
    /// Создаёт контроллер с замоканным <see cref="IWeatherService"/> и подставленным пользователем.
    /// </summary>
    private static WeatherController CreateController(Mock<IWeatherService> mockService, Guid? userId = null)
    {
        var controller = new WeatherController(mockService.Object);
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

    [Fact]
    public async Task GetWeather_ValidCity_ReturnsOkWithWeather()
    {
        var userId = Guid.NewGuid();
        var city = "Moscow";
        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        var expectedWeather = new WeatherSnapshotDto
        {
            City = city,
            Date = date,
            TemperatureCelsius = 15,
            Condition = "Clouds",
            HumidityPercent = 70,
            Precipitation = "none"
        };

        var mockService = new Mock<IWeatherService>();
        mockService.Setup(s => s.GetWeatherAsync(city, date, It.IsAny<CancellationToken>()))
                   .ReturnsAsync(expectedWeather);

        var controller = CreateController(mockService, userId);
        var result = await controller.GetWeather(city, date, CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var actualWeather = Assert.IsType<WeatherSnapshotDto>(okResult.Value);
        Assert.Equal(expectedWeather.City, actualWeather.City);
        Assert.Equal(expectedWeather.TemperatureCelsius, actualWeather.TemperatureCelsius);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task GetWeather_EmptyCity_ReturnsBadRequest(string city)
    {
        var mockService = new Mock<IWeatherService>();
        var controller = CreateController(mockService, Guid.NewGuid());

        var result = await controller.GetWeather(city, DateOnly.FromDateTime(DateTime.UtcNow), CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetWeather_FutureDate_ReturnsBadRequest()
    {
        var mockService = new Mock<IWeatherService>();
        var controller = CreateController(mockService, Guid.NewGuid());
        var futureDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2));

        var result = await controller.GetWeather("Moscow", futureDate, CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetWeather_CityNotFound_ReturnsNotFound()
    {
        var userId = Guid.NewGuid();
        var city = "UnknownCity";
        var date = DateOnly.FromDateTime(DateTime.UtcNow);

        var mockService = new Mock<IWeatherService>();
        mockService.Setup(s => s.GetWeatherAsync(city, date, It.IsAny<CancellationToken>()))
                   .ThrowsAsync(new KeyNotFoundException());

        var controller = CreateController(mockService, userId);
        var result = await controller.GetWeather(city, date, CancellationToken.None);

        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetWeather_ServiceThrowsArgumentException_ReturnsBadRequest()
    {
        var userId = Guid.NewGuid();
        var city = "Moscow";
        var date = DateOnly.FromDateTime(DateTime.UtcNow);

        var mockService = new Mock<IWeatherService>();
        mockService.Setup(s => s.GetWeatherAsync(city, date, It.IsAny<CancellationToken>()))
                   .ThrowsAsync(new ArgumentException("City name too long"));

        var controller = CreateController(mockService, userId);
        var result = await controller.GetWeather(city, date, CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetWeather_ServiceThrowsTooManyRequests_ReturnsTooManyRequests()
    {
        var userId = Guid.NewGuid();
        var city = "ratelimit";
        var date = DateOnly.FromDateTime(DateTime.UtcNow);

        var mockService = new Mock<IWeatherService>();
        mockService.Setup(s => s.GetWeatherAsync(city, date, It.IsAny<CancellationToken>()))
                   .ThrowsAsync(new HttpRequestException(
                       "Too many requests to weather API.",
                       null,
                       HttpStatusCode.TooManyRequests));

        var controller = CreateController(mockService, userId);
        var result = await controller.GetWeather(city, date, CancellationToken.None);

        var statusResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(StatusCodes.Status429TooManyRequests, statusResult.StatusCode);
    }

    // Тест GetWeather_Unauthenticated_ReturnsUnauthorized удалён, так как
    // атрибут [Authorize] на контроллере не вызывает исключение в модульных тестах,
    // а возвращает ChallengeResult, что требует интеграционного тестирования.
}
