using System.Net;
using HabitApi.Models.DTO;
using HabitApi.Services.Interfaces;

namespace HabitApi.Tests.Integration.Infrastructure;

internal sealed class TestWeatherService : IWeatherService
{
    public Task<WeatherSnapshotDto> GetWeatherAsync(string city, DateOnly date, CancellationToken cancellationToken)
    {
        if (string.Equals(city, "InvalidCity", StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromException<WeatherSnapshotDto>(
                new KeyNotFoundException($"Weather data for city '{city}' not found."));
        }

        if (string.Equals(city, "ratelimit", StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromException<WeatherSnapshotDto>(
                new HttpRequestException(
                    "Too many requests to weather API.",
                    null,
                    HttpStatusCode.TooManyRequests));
        }

        return Task.FromResult(new WeatherSnapshotDto
        {
            City = city,
            Date = date,
            Condition = "Clouds",
            TemperatureCelsius = 18,
            HumidityPercent = 60,
            Precipitation = "none"
        });
    }
}
