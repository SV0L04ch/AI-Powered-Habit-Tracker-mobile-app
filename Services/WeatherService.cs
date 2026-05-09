using System.Text.Json;
using HabitApi.Models.DTO;
using HabitApi.Services.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace HabitApi.Services;

public sealed class WeatherService : IWeatherService
{
    private readonly IDistributedCache _cache;
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _baseUrl;

    public WeatherService(IDistributedCache cache, HttpClient httpClient, IConfiguration configuration)
    {
        _cache = cache;
        _httpClient = httpClient;
        _apiKey = Environment.GetEnvironmentVariable("WEATHER_API_KEY")
                  ?? configuration["WeatherApi:ApiKey"]
                  ?? throw new InvalidOperationException("WEATHER_API_KEY is not configured.");
        _baseUrl = configuration["WeatherApi:BaseUrl"]
                   ?? "https://api.openweathermap.org/data/2.5";
    }

    public async Task<WeatherSnapshotDto> GetWeatherAsync(string city, DateOnly date, CancellationToken cancellationToken)
    {
        string cacheKey = $"weather:{city}:{date:yyyyMMdd}";

        // 1. Проверяем кэш Redis
        var cached = await _cache.GetStringAsync(cacheKey, cancellationToken);
        if (cached is not null)
            return JsonSerializer.Deserialize<WeatherSnapshotDto>(cached)!;

        // 2. Если нет в кэше – запрос к OpenWeatherMap
        string url = $"{_baseUrl}/weather?q={Uri.EscapeDataString(city)}&appid={_apiKey}&units=metric";

        HttpResponseMessage response = await _httpClient.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        var weather = root.GetProperty("weather")[0];
        var mainInfo = root.GetProperty("main");

        var snapshot = new WeatherSnapshotDto
        {
            City = city,
            Date = date,
            Condition = weather.GetProperty("main").GetString() ?? "Unknown",
            TemperatureCelsius = (int)Math.Round(mainInfo.GetProperty("temp").GetDouble()),
            HumidityPercent = mainInfo.TryGetProperty("humidity", out var humidity) ? humidity.GetInt32() : null,
            Precipitation = root.TryGetProperty("rain", out _)
                ? "rain"
                : root.TryGetProperty("snow", out _)
                    ? "snow"
                    : "none"
        };

        // 3. Сохраняем в Redis на 3 часа
        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(3)
        };
        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(snapshot), cacheOptions, cancellationToken);

        return snapshot;
    }
}
