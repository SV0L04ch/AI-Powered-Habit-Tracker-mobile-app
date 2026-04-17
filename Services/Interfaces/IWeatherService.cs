using HabitApi.Models.DTO;

namespace HabitApi.Services.Interfaces;

public interface IWeatherService
{
    Task<WeatherSnapshotDto> GetWeatherAsync(string city, DateOnly date, CancellationToken cancellationToken);
}
