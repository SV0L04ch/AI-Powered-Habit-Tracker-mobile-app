using HabitApi.Models.DTO;
using HabitApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HabitApi.Controllers;

/// <summary>
/// Контроллер для статистики и сводок.
/// Предоставляет персональную ежедневную сводку и анонимную городскую статистику.
/// </summary>
[ApiController]
[Route("api/stats")]
public sealed class StatsController : ControllerBase
{
    private readonly IStatsService _statsService;

    public StatsController(IStatsService statsService)
    {
        _statsService = statsService;
    }

    /// <summary>
    /// Получить ежедневную персональную сводку для текущего пользователя.
    /// </summary>
    /// <param name="date">Дата сводки.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Сводка с выполненными привычками, погодой и ИИ-комментарием.</returns>
    /// <response code="200">Сводка успешно получена.</response>
    /// <response code="400">Некорректная дата.</response>
    /// <response code="401">Пользователь не авторизован.</response>
    /// <response code="404">Пользователь не найден.</response>
    [Authorize]
    [HttpGet("daily-summary")]
    [ProducesResponseType(typeof(DailySummaryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DailySummaryDto>> GetDailySummary(
        DateOnly? date,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var targetDate = date ?? DateOnly.FromDateTime(DateTime.UtcNow);
        
        if (targetDate > DateOnly.FromDateTime(DateTime.UtcNow))
            return BadRequest(new { error = "Cannot get summary for future date." });

        try
        {
            var summary = await _statsService.GetDailySummaryAsync(userId, targetDate, cancellationToken);
            return Ok(summary);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { error = "User not found." });
        }
    }

    /// <summary>
    /// Получить анонимную сводку по городу (без привязки к пользователю).
    /// Сводка формируется раз в неделю и кэшируется.
    /// </summary>
    /// <param name="city">Название города (обязательно).</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Анонимная статистика привычек в городе за последнюю неделю.</returns>
    /// <response code="200">Сводка успешно получена.</response>
    /// <response code="400">Город не указан.</response>
    [AllowAnonymous]
    [HttpGet("city-summary")]
    [ProducesResponseType(typeof(CitySummaryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CitySummaryDto>> GetCitySummary(
        string city,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(city))
            return BadRequest(new { error = "City parameter is required." });

        var summary = await _statsService.GetWeeklyCitySummaryAsync(city, cancellationToken);
        return Ok(summary);
    }

    /// <summary>
    /// Извлекает идентификатор текущего пользователя из JWT-токена.
    /// </summary>
    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userIdClaim is null)
            throw new UnauthorizedAccessException("User ID not found in token.");
        return Guid.Parse(userIdClaim);
    }
}
