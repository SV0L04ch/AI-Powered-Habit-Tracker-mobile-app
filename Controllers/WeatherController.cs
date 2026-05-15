using HabitApi.Models.DTO;
using HabitApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HabitApi.Controllers;

/// <summary>
/// Контроллер для получения погодных данных.
/// Требует аутентификации для предотвращения злоупотреблений.
/// </summary>
[ApiController]
[Route("api/weather")]
[Authorize]
public sealed class WeatherController : ControllerBase
{
    private readonly IWeatherService _weatherService;

    public WeatherController(IWeatherService weatherService)
    {
        _weatherService = weatherService;
    }

    /// <summary>
    /// Получить погоду для указанного города на указанную дату.
    /// Данные кешируются на сервере, обновление не реже одного раза в 3 часа.
    /// </summary>
    /// <param name="city">Название города (обязательно).</param>
    /// <param name="date">Дата.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Снимок погоды: температура, осадки, облачность.</returns>
    /// <response code="200">Погода успешно получена.</response>
    /// <response code="400">Не указан город, превышена длина названия или дата в будущем.</response>
    /// <response code="401">Пользователь не авторизован.</response>
    /// <response code="404">Город не найден.</response>
    [HttpGet]
    [ProducesResponseType(typeof(WeatherSnapshotDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WeatherSnapshotDto>> GetWeather(
        string city,
        DateOnly? date,
        CancellationToken cancellationToken)
    {
        // Валидация города
        if (string.IsNullOrWhiteSpace(city))
            return BadRequest(new { error = "City parameter is required." });

        if (city.Length > 100)
            return BadRequest(new { error = "City name is too long." });

        var targetDate = date ?? DateOnly.FromDateTime(DateTime.UtcNow);

        // Запрещаем будущие даты
        if (targetDate > DateOnly.FromDateTime(DateTime.UtcNow))
            return BadRequest(new { error = "Cannot get weather for future date." });

        try
        {
            var snapshot = await _weatherService.GetWeatherAsync(city, targetDate, cancellationToken);
            return Ok(snapshot);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { error = $"Weather data for city '{city}' not found." });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        // Остальные исключения обрабатываются глобальным фильтром
    }
}
