using RichardSzalay.MockHttp;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using HabitApi.Services;
using System.Net;
using System.Text.Json;
using Xunit;
using Moq;

namespace HabitApi.Tests.Services;

public class WeatherServiceTests
{
    // Предположим, что WeatherService принимает HttpClient и IMemoryCache
    private static WeatherService CreateService(HttpClient httpClient)
    {
        var cacheOptions = Options.Create(new MemoryCacheOptions());
        var cache = new MemoryCache(cacheOptions);
        return new WeatherService(httpClient, cache);
    }

    [Fact]
    public async Task GetWeatherAsync_ValidCity_ReturnsParsedWeatherData()
    {
        // Arrange
        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When("https://api.openweathermap.org/*")
                .Respond("application/json",
                @"{
                    ""main"": { ""temp"": 15.2 },
                    ""weather"": [ { ""description"": ""cloudy"" } ]
                  }");

        var httpClient = mockHttp.ToHttpClient();
        var service = CreateService(httpClient);

        // Act
        var result = await service.GetWeatherAsync("Moscow");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(15.2, result.Temperature);    // зависит от модели ответа
        Assert.Equal("cloudy", result.Description);
    }

    [Fact]
    public async Task GetWeatherAsync_ApiReturnsError_ThrowsOrReturnsFallback()
    {
        // Arrange
        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When("https://api.openweathermap.org/*")
                .Respond(HttpStatusCode.NotFound); // имитация ошибки

        var httpClient = mockHttp.ToHttpClient();
        var service = CreateService(httpClient);

        // Act & Assert: зависит от реализации.
        // Если метод пробрасывает исключение:
        await Assert.ThrowsAsync<HttpRequestException>(() => service.GetWeatherAsync("InvalidCity"));

        // Если метод возвращает null или объект с ошибкой, проверь соответствующее поведение.
    }

    [Fact]
    public async Task GetWeatherAsync_SameCitySecondCall_UsesCache()
    {
        // Arrange
        var mockHttp = new MockHttpMessageHandler();
        var request = mockHttp.When("https://api.openweathermap.org/*")
                              .Respond("application/json", @"{ ""main"": { ""temp"": 10.0 }, ""weather"": [ { ""description"": ""sunny"" } ] }");

        var httpClient = mockHttp.ToHttpClient();
        var service = CreateService(httpClient);

        // Act
        var firstResult = await service.GetWeatherAsync("London");
        var secondResult = await service.GetWeatherAsync("London");

        // Assert
        Assert.Equal(firstResult.Temperature, secondResult.Temperature);
        // Убедимся, что HTTP-запрос был выполнен только один раз
        Assert.Equal(1, mockHttp.GetMatchCount(request));
    }
}