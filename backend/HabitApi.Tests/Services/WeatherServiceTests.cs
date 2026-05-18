using System.Net;
using Xunit;
using Moq;
using RichardSzalay.MockHttp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;
using HabitApi.Services;
using HabitApi.Models.DTO;

namespace HabitApi.Tests.Services;

/// <summary>
/// Модульные тесты для <see cref="WeatherService"/>.
/// Проверяет получение погодных данных, кэширование в Redis и обработку ошибок.
/// </summary>
public class WeatherServiceTests
{
    private static WeatherService CreateService(
        HttpClient httpClient,
        IDistributedCache? cache = null,
        IConfiguration? config = null,
        ILogger<WeatherService>? logger = null)
    {
        cache ??= Mock.Of<IDistributedCache>();
        config ??= new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            { "WeatherApi:ApiKey", "test-key" }
        }).Build();
        logger ??= Mock.Of<ILogger<WeatherService>>();

        return new WeatherService(cache, httpClient, config, logger);
    }

    [Fact]
    public async Task GetWeatherAsync_ValidCity_ReturnsParsedWeatherData()
    {
        var city = "Moscow";
        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When("https://api.openweathermap.org/data/2.5/weather*")
                .Respond("application/json", @"{
                    ""main"": { ""temp"": 15.2, ""humidity"": 70 },
                    ""weather"": [ { ""main"": ""Clouds"" } ],
                    ""rain"": { ""1h"": 0.5 }
                }");

        var service = CreateService(mockHttp.ToHttpClient());

        var result = await service.GetWeatherAsync(city, date, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(city, result.City);
        Assert.Equal(date, result.Date);
        Assert.Equal(15, result.TemperatureCelsius);
        Assert.Equal(70, result.HumidityPercent);
        Assert.Equal("Clouds", result.Condition);
        Assert.Equal("rain", result.Precipitation);
    }

    [Fact]
    public async Task GetWeatherAsync_SameCitySecondCall_UsesCache()
    {
        var city = "London";
        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        var cacheKey = $"HabitTracker_weather_{city}_{date:yyyyMMdd}";

        var mockCache = new Mock<IDistributedCache>();
        var cachedSnapshot = new WeatherSnapshotDto
        {
            City = city,
            Date = date,
            Condition = "Sunny",
            TemperatureCelsius = 20
        };
        var cachedJson = JsonSerializer.Serialize(cachedSnapshot);

        var firstCall = true;
        mockCache.Setup(c => c.GetAsync(cacheKey, It.IsAny<CancellationToken>()))
                 .ReturnsAsync(() =>
                 {
                     if (firstCall)
                     {
                         firstCall = false;
                         return null;
                     }
                     return Encoding.UTF8.GetBytes(cachedJson);
                 });

        var mockHttp = new MockHttpMessageHandler();
        var request = mockHttp.When("https://api.openweathermap.org/data/2.5/weather*")
                              .Respond("application/json", @"{
                                  ""main"": { ""temp"": 25.0 },
                                  ""weather"": [ { ""main"": ""Sunny"" } ]
                              }");

        var httpClient = mockHttp.ToHttpClient();
        var service = CreateService(httpClient, mockCache.Object);

        var firstResult = await service.GetWeatherAsync(city, date, CancellationToken.None);
        var secondResult = await service.GetWeatherAsync(city, date, CancellationToken.None);

        Assert.Equal(25, firstResult.TemperatureCelsius);
        Assert.Equal(20, secondResult.TemperatureCelsius);
        Assert.Equal(1, mockHttp.GetMatchCount(request));
    }

    /// <summary>
    /// Проверяет, что при ошибке API возвращается fallback-объект (Service unavailable), а не исключение.
    /// </summary>
    [Fact]
    public async Task GetWeatherAsync_ApiError_ReturnsFallback()
    {
        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When("https://api.openweathermap.org/data/2.5/weather*")
                .Respond(System.Net.HttpStatusCode.InternalServerError);

        var service = CreateService(mockHttp.ToHttpClient());

        var result = await service.GetWeatherAsync("City", DateOnly.FromDateTime(DateTime.UtcNow), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("Service unavailable", result.Condition);
    }

    [Fact]
    public async Task GetWeatherAsync_CityNotFound_ThrowsKeyNotFoundException()
    {
        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When("https://api.openweathermap.org/data/2.5/weather*")
                .Respond(HttpStatusCode.NotFound, "application/json", @"{ ""cod"": 404, ""message"": ""city not found"" }");

        var service = CreateService(mockHttp.ToHttpClient());

        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => service.GetWeatherAsync("InvalidCity", DateOnly.FromDateTime(DateTime.UtcNow), CancellationToken.None));
    }

    [Fact]
    public async Task GetWeatherAsync_RateLimited_ThrowsTooManyRequestsException()
    {
        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When("https://api.openweathermap.org/data/2.5/weather*")
                .Respond(HttpStatusCode.TooManyRequests, "application/json", @"{ ""cod"": 429, ""message"": ""rate limit exceeded"" }");

        var service = CreateService(mockHttp.ToHttpClient());

        var exception = await Assert.ThrowsAsync<HttpRequestException>(
            () => service.GetWeatherAsync("ratelimit", DateOnly.FromDateTime(DateTime.UtcNow), CancellationToken.None));
        Assert.Equal(HttpStatusCode.TooManyRequests, exception.StatusCode);
    }

    [Fact]
    public async Task GetWeatherAsync_RedisUnavailable_FallsBackToApi()
    {
        var city = "Berlin";
        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        var mockCache = new Mock<IDistributedCache>();
        mockCache.Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                 .ThrowsAsync(new Exception("Redis connection failed"));

        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When("https://api.openweathermap.org/data/2.5/weather*")
                .Respond("application/json", @"{
                    ""main"": { ""temp"": 18.0 },
                    ""weather"": [ { ""main"": ""Clear"" } ]
                }");

        var service = CreateService(mockHttp.ToHttpClient(), mockCache.Object);

        var result = await service.GetWeatherAsync(city, date, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(18, result.TemperatureCelsius);
        Assert.Equal("Clear", result.Condition);
    }

    [Fact]
    public async Task GetWeatherAsync_HistoricalDate_ThrowsArgumentException()
    {
        var service = CreateService(new MockHttpMessageHandler().ToHttpClient());
        var historicalDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1));

        await Assert.ThrowsAsync<ArgumentException>(
            () => service.GetWeatherAsync("Moscow", historicalDate, CancellationToken.None));
    }
}
