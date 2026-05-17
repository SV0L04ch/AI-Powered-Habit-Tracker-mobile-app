using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using HabitApi.Controllers;
using HabitApi.Models.DTO;
using HabitApi.Services.Interfaces;

namespace HabitApi.Tests.Controllers;

public class WeatherControllerTests
{
    [Fact]
    public async Task GetWeather_ValidCity_ReturnsOkWithWeatherData()
    {
        // Arrange
        var city = "Москва";
        var expectedWeather = new WeatherDto
        {
            Temperature = 15.2,
            Description = "облачно"
        };

        var mockService = new Mock<IWeatherService>();
        mockService.Setup(s => s.GetWeatherAsync(city))
                   .ReturnsAsync(expectedWeather);

        var controller = new WeatherController(mockService.Object);

        // Act
        var result = await controller.GetWeather(city);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var actualWeather = Assert.IsType<WeatherDto>(okResult.Value);
        Assert.Equal(expectedWeather.Temperature, actualWeather.Temperature);
        Assert.Equal(expectedWeather.Description, actualWeather.Description);
    }

    [Fact]
    public async Task GetWeather_ServiceReturnsNull_ReturnsNotFound()
    {
        // Arrange
        var city = "НесуществующийГород";
        var mockService = new Mock<IWeatherService>();
        mockService.Setup(s => s.GetWeatherAsync(city))
                   .ReturnsAsync((WeatherDto?)null);

        var controller = new WeatherController(mockService.Object);

        // Act
        var result = await controller.GetWeather(city);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetWeather_ServiceThrowsException_ReturnsBadRequest()
    {
        // Arrange
        var city = "ErrorCity";
        var mockService = new Mock<IWeatherService>();
        mockService.Setup(s => s.GetWeatherAsync(city))
                   .ThrowsAsync(new HttpRequestException("API error"));

        var controller = new WeatherController(mockService.Object);

        // Act
        var result = await controller.GetWeather(city);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("API error", badRequest.Value);
    }
}