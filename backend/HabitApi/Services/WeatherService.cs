using System.Net;
using System.Text.Json;
using HabitApi.Models.DTO;
using HabitApi.Services.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace HabitApi.Services;

/// <summary>
/// Сервис для получения погодных данных с OpenWeatherMap и кэширования их в Redis.
/// </summary>
public sealed class WeatherService : IWeatherService
{
    private const string CacheKeyPrefix = "HabitTracker_weather_";
    private static readonly TimeSpan DefaultCacheDuration = TimeSpan.FromHours(3);

    private readonly IDistributedCache _cache;
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _baseUrl;
    private readonly ILogger<WeatherService> _logger;

    public WeatherService(
        IDistributedCache cache,
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<WeatherService> logger)
    {
        _cache = cache;
        _httpClient = httpClient;
        _logger = logger;
        _apiKey = Environment.GetEnvironmentVariable("WEATHER_API_KEY")
                  ?? configuration["WeatherApi:ApiKey"]
                  ?? throw new InvalidOperationException("WEATHER_API_KEY is not configured.");
        _baseUrl = configuration["WeatherApi:BaseUrl"]
                   ?? "https://api.openweathermap.org/data/2.5";
    }

    /// <summary>
    /// Получает снимок погоды для указанного города и даты.
    /// При возможности берёт данные из кэша Redis, иначе запрашивает OpenWeatherMap.
    /// Поддерживается только текущая дата (исторические данные недоступны).
    /// При ошибке API возвращает fallback-объект с пометкой о недоступности сервиса.
    /// </summary>
    public async Task<WeatherSnapshotDto> GetWeatherAsync(string city, DateOnly date, CancellationToken cancellationToken)
    {
        // 1. Запрещаем исторические даты
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        if (date != today)
            throw new ArgumentException("Historical weather data is not supported.");

        var cacheKey = $"{CacheKeyPrefix}{city}_{date:yyyyMMdd}";

        // 2. Пытаемся получить из кэша
        try
        {
            var cached = await _cache.GetStringAsync(cacheKey, cancellationToken);
            if (!string.IsNullOrWhiteSpace(cached))
            {
                try
                {
                    var deserialized = JsonSerializer.Deserialize<WeatherSnapshotDto>(cached);
                    if (deserialized is not null)
                    {
                        _logger.LogDebug("Cache hit for weather {City} on {Date}", city, date);
                        return deserialized;
                    }
                }
                catch (JsonException)
                {
                    _logger.LogWarning("Corrupted weather cache for {City} on {Date}, will refresh", city, date);
                    await _cache.RemoveAsync(cacheKey, cancellationToken);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Redis unavailable, fetching weather directly");
        }

        // 3. Запрос к OpenWeatherMap
        var url = $"{_baseUrl}/weather?q={Uri.EscapeDataString(city)}&appid={_apiKey}&units=metric";

        WeatherSnapshotDto snapshot;
        try
        {
            var response = await _httpClient.GetAsync(url, cancellationToken);

            if (response.StatusCode == HttpStatusCode.NotFound)
                throw new KeyNotFoundException($"Weather data for city '{city}' not found.");

            if (response.StatusCode == HttpStatusCode.TooManyRequests)
                throw new HttpRequestException(
                    "Too many requests to weather API.",
                    null,
                    HttpStatusCode.TooManyRequests);

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            snapshot = ParseWeatherResponse(city, date, json);
        }
        catch (Exception ex) when (ex is TaskCanceledException ||
                                   ex is HttpRequestException { StatusCode: not HttpStatusCode.TooManyRequests })
        {
            _logger.LogWarning(ex, "Weather API unavailable for {City}, returning fallback", city);
            snapshot = CreateFallback(city, date);
        }

        // 4. Сохраняем в кэш
        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = DefaultCacheDuration
        };

        try
        {
            var serialized = JsonSerializer.Serialize(snapshot);
            await _cache.SetStringAsync(cacheKey, serialized, cacheOptions, cancellationToken);
            _logger.LogDebug("Cached weather for {City} on {Date}", city, date);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to cache weather for {City} on {Date}", city, date);
        }

        return snapshot;
    }

    /// <summary>
    /// Создаёт fallback-объект погоды, когда API недоступен.
    /// </summary>
    private static WeatherSnapshotDto CreateFallback(string city, DateOnly date)
    {
        return new WeatherSnapshotDto
        {
            City = city,
            Date = date,
            Condition = "Service unavailable",
            TemperatureCelsius = 0,
            HumidityPercent = null,
            Precipitation = "unknown"
        };
    }

    /// <summary>
    /// Разбирает JSON-ответ от OpenWeatherMap.
    /// </summary>
    private static WeatherSnapshotDto ParseWeatherResponse(string city, DateOnly date, string json)
    {
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        var condition = "Unknown";
        var temperatureCelsius = 0;
        int? humidityPercent = null;
        string? precipitation = null;

        if (root.TryGetProperty("weather", out var weatherArray) &&
            weatherArray.ValueKind == JsonValueKind.Array &&
            weatherArray.GetArrayLength() > 0)
        {
            var firstWeather = weatherArray[0];
            if (firstWeather.TryGetProperty("main", out var mainProp))
                condition = mainProp.GetString() ?? "Unknown";
        }

        if (root.TryGetProperty("main", out var mainInfo))
        {
            if (mainInfo.TryGetProperty("temp", out var tempProp) && tempProp.TryGetDouble(out var temp))
                temperatureCelsius = (int)Math.Round(temp);
            if (mainInfo.TryGetProperty("humidity", out var humProp) && humProp.TryGetInt32(out var hum))
                humidityPercent = hum;
        }

        if (root.TryGetProperty("rain", out _))
            precipitation = "rain";
        else if (root.TryGetProperty("snow", out _))
            precipitation = "snow";
        else
            precipitation = "none";

        return new WeatherSnapshotDto
        {
            City = city,
            Date = date,
            Condition = condition,
            TemperatureCelsius = temperatureCelsius,
            HumidityPercent = humidityPercent,
            Precipitation = precipitation
        };
    }
}
