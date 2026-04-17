using HabitApi.Models.DTO;
using HabitApi.Services.Interfaces;

namespace HabitApi.Services;

/// <summary>
/// Сервис для получения погодных данных (заглушка).
/// В реальном проекте здесь должен быть вызов OpenWeatherMap API с кэшированием.
/// </summary>
public sealed class WeatherService : IWeatherService
{
    /// <inheritdoc />
    public Task<WeatherSnapshotDto> GetWeatherAsync(string city, DateOnly date, CancellationToken cancellationToken)
    {
        // TODO: Реализовать реальный запрос к API погоды с учётом кэширования (НФТ: обновление не реже 1 раза в 3 часа)
        var snapshot = new WeatherSnapshotDto
        {
            City = city,
            Date = date,
            Condition = "clear",
            TemperatureCelsius = 22,
            HumidityPercent = 65,
            Precipitation = "none"
        };
        return Task.FromResult(snapshot);
    }
}
