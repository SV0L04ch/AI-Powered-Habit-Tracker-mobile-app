using System.Globalization;
using System.Net;
using System.Text.Json;
using HabitApi.Models.DTO;
using HabitApi.Services.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace HabitApi.Services;

public sealed class WeatherService : IWeatherService
{
    private const string GeocodingBaseUrl = "https://api.openweathermap.org/geo/1.0";
    private readonly IDistributedCache _cache;
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _weatherBaseUrl;

    public WeatherService(IDistributedCache cache, HttpClient httpClient, IConfiguration configuration)
    {
        _cache = cache;
        _httpClient = httpClient;
        _apiKey = Environment.GetEnvironmentVariable("WEATHER_API_KEY")
                  ?? configuration["WeatherApi:ApiKey"]
                  ?? throw new InvalidOperationException("WEATHER_API_KEY is not configured.");
        _weatherBaseUrl = (configuration["WeatherApi:BaseUrl"]
                           ?? Environment.GetEnvironmentVariable("WEATHER_BASE_URL")
                           ?? "https://api.openweathermap.org/data/2.5").TrimEnd('/');
    }

    public async Task<WeatherSnapshotDto> GetWeatherAsync(string city, DateOnly date, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(city))
            throw new ArgumentException("City is required.", nameof(city));

        if (date > DateOnly.FromDateTime(DateTime.UtcNow))
            throw new ArgumentException("Cannot get weather for future date.");

        var normalizedCity = city.Trim();
        var cacheKey = $"weather:{normalizedCity.ToLowerInvariant()}:{date:yyyyMMdd}";
        var cached = await _cache.GetStringAsync(cacheKey, cancellationToken);
        if (cached is not null)
            return JsonSerializer.Deserialize<WeatherSnapshotDto>(cached)!;

        var snapshot = date == DateOnly.FromDateTime(DateTime.UtcNow)
            ? await GetCurrentWeatherSnapshotAsync(normalizedCity, date, cancellationToken)
            : await GetHistoricalWeatherSnapshotAsync(normalizedCity, date, cancellationToken);

        await _cache.SetStringAsync(
            cacheKey,
            JsonSerializer.Serialize(snapshot),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(3)
            },
            cancellationToken);

        return snapshot;
    }

    private async Task<WeatherSnapshotDto> GetCurrentWeatherSnapshotAsync(string city, DateOnly date, CancellationToken cancellationToken)
    {
        var url = $"{_weatherBaseUrl}/weather?q={Uri.EscapeDataString(city)}&appid={_apiKey}&units=metric";
        using var document = await SendOpenWeatherRequestAsync(
            url,
            $"Weather data for city '{city}' not found.",
            "OpenWeather current weather access is not available with the configured API key.",
            cancellationToken);

        var root = document.RootElement;
        var weather = root.GetProperty("weather")[0];
        var main = root.GetProperty("main");

        return new WeatherSnapshotDto
        {
            City = city,
            Date = date,
            Condition = weather.GetProperty("main").GetString() ?? "Unknown",
            TemperatureCelsius = (int)Math.Round(main.GetProperty("temp").GetDouble()),
            HumidityPercent = main.TryGetProperty("humidity", out var humidity) ? humidity.GetInt32() : null,
            Precipitation = ExtractPrecipitation(root)
        };
    }

    private async Task<WeatherSnapshotDto> GetHistoricalWeatherSnapshotAsync(string city, DateOnly date, CancellationToken cancellationToken)
    {
        var coordinates = await ResolveCoordinatesAsync(city, cancellationToken);
        var unixTime = new DateTimeOffset(date.ToDateTime(new TimeOnly(12, 0), DateTimeKind.Utc)).ToUnixTimeSeconds();
        var url =
            $"{_weatherBaseUrl}/onecall/timemachine?lat={coordinates.Latitude.ToString(CultureInfo.InvariantCulture)}&lon={coordinates.Longitude.ToString(CultureInfo.InvariantCulture)}&dt={unixTime}&appid={_apiKey}&units=metric&only_current=true";

        using var document = await SendOpenWeatherRequestAsync(
            url,
            $"Historical weather data for city '{city}' and date '{date:yyyy-MM-dd}' not found.",
            "Historical weather data is unavailable with the configured OpenWeather access.",
            cancellationToken);

        var observation = GetHistoricalObservation(document.RootElement);
        var weather = observation.GetProperty("weather")[0];

        return new WeatherSnapshotDto
        {
            City = city,
            Date = date,
            Condition = weather.GetProperty("main").GetString() ?? "Unknown",
            TemperatureCelsius = (int)Math.Round(observation.GetProperty("temp").GetDouble()),
            HumidityPercent = observation.TryGetProperty("humidity", out var humidity) ? humidity.GetInt32() : null,
            Precipitation = ExtractPrecipitation(observation)
        };
    }

    private async Task<(double Latitude, double Longitude)> ResolveCoordinatesAsync(string city, CancellationToken cancellationToken)
    {
        var cacheKey = $"weather:geo:{city.ToLowerInvariant()}";
        var cached = await _cache.GetStringAsync(cacheKey, cancellationToken);
        if (!string.IsNullOrWhiteSpace(cached))
        {
            var parts = cached.Split(';');
            if (parts.Length == 2
                && double.TryParse(parts[0], CultureInfo.InvariantCulture, out var cachedLat)
                && double.TryParse(parts[1], CultureInfo.InvariantCulture, out var cachedLon))
            {
                return (cachedLat, cachedLon);
            }
        }

        var url = $"{GeocodingBaseUrl}/direct?q={Uri.EscapeDataString(city)}&limit=1&appid={_apiKey}";
        using var document = await SendOpenWeatherRequestAsync(
            url,
            $"Weather data for city '{city}' not found.",
            "OpenWeather geocoding access is not available with the configured API key.",
            cancellationToken);

        if (document.RootElement.ValueKind != JsonValueKind.Array || document.RootElement.GetArrayLength() == 0)
            throw new KeyNotFoundException($"Weather data for city '{city}' not found.");

        var location = document.RootElement[0];
        var latitude = location.GetProperty("lat").GetDouble();
        var longitude = location.GetProperty("lon").GetDouble();

        await _cache.SetStringAsync(
            cacheKey,
            $"{latitude.ToString(CultureInfo.InvariantCulture)};{longitude.ToString(CultureInfo.InvariantCulture)}",
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(30)
            },
            cancellationToken);

        return (latitude, longitude);
    }

    private async Task<JsonDocument> SendOpenWeatherRequestAsync(
        string url,
        string notFoundMessage,
        string accessDeniedMessage,
        CancellationToken cancellationToken)
    {
        using var response = await _httpClient.GetAsync(url, cancellationToken);
        var payload = await response.Content.ReadAsStringAsync(cancellationToken);

        if (response.IsSuccessStatusCode)
            return JsonDocument.Parse(payload);

        if (response.StatusCode == HttpStatusCode.NotFound)
            throw new KeyNotFoundException(notFoundMessage);

        if (response.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden)
            throw new ArgumentException(accessDeniedMessage);

        throw new HttpRequestException(
            $"OpenWeather request failed with status code {(int)response.StatusCode}: {payload}",
            null,
            response.StatusCode);
    }

    private static JsonElement GetHistoricalObservation(JsonElement root)
    {
        if (root.TryGetProperty("current", out var current))
            return current;

        if (root.TryGetProperty("data", out var data)
            && data.ValueKind == JsonValueKind.Array
            && data.GetArrayLength() > 0)
        {
            return data[0];
        }

        throw new KeyNotFoundException("Historical weather response did not contain an observation.");
    }

    private static string ExtractPrecipitation(JsonElement element)
    {
        if (element.TryGetProperty("rain", out _))
            return "rain";

        if (element.TryGetProperty("snow", out _))
            return "snow";

        return "none";
    }
}
