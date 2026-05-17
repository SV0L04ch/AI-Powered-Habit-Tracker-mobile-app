using HabitApi.Models.DTO;
using HabitApi.Services.Interfaces;

namespace HabitApi.Tests.Integration.Infrastructure;

internal sealed class TestWeatherService : IWeatherService
{
    public Task<WeatherSnapshotDto> GetWeatherAsync(string city, DateOnly date, CancellationToken cancellationToken)
    {
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
