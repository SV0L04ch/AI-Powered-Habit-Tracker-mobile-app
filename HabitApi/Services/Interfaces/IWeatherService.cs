using HabitApi.Models.DTO;

namespace HabitApi.Services.Interfaces;

/// <summary>
/// Сервис для получения погодных данных.
/// </summary>
public interface IWeatherService
{
    /// <summary>
    /// Получает снимок погоды для указанного города на заданную дату.
    /// При возможности использует кэширование, чтобы уменьшить количество запросов к внешнему API.
    /// </summary>
    /// <param name="city">Название города (обязательно).</param>
    /// <param name="date">Дата, за которую запрашивается погода (не может быть в будущем).</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Снимок погоды с температурой, осадками и влажностью.</returns>
    /// <exception cref="ArgumentException">Если город не указан или дата в будущем.</exception>
    /// <exception cref="HttpRequestException">При недоступности внешнего API погоды.</exception>
    Task<WeatherSnapshotDto> GetWeatherAsync(string city, DateOnly date, CancellationToken cancellationToken);
}
