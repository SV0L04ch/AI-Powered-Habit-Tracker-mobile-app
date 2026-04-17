using HabitApi.Models.DTO;
using HabitApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HabitApi.Controllers;

/// <summary>
/// Контроллер для управления отметками выполнения привычек.
/// </summary>
[ApiController]
[Route("api/habits/{habitId:guid}/entries")]
[Authorize] // Требуем аутентификацию
public sealed class HabitEntriesController : ControllerBase
{
    private readonly IHabitEntryService _habitEntryService;

    public HabitEntriesController(IHabitEntryService habitEntryService)
    {
        _habitEntryService = habitEntryService;
    }

    /// <summary>
    /// Получить отметки выполнения для указанной привычки за период.
    /// </summary>
    /// <param name="habitId">Идентификатор привычки.</param>
    /// <param name="fromDate">Начальная дата (опционально).</param>
    /// <param name="toDate">Конечная дата (опционально).</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Коллекция отметок.</returns>
    /// <response code="200">Успешное получение.</response>
    /// <response code="401">Пользователь не авторизован.</response>
    /// <response code="404">Привычка не найдена или не принадлежит пользователю.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<HabitEntryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyCollection<HabitEntryDto>>> GetEntries(
        Guid habitId,
        [FromQuery] DateOnly? fromDate,
        [FromQuery] DateOnly? toDate,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId(); // извлекаем userId из токена
        var entries = await _habitEntryService.GetHabitEntriesAsync(userId, habitId, fromDate, toDate, cancellationToken);
        return Ok(entries);
    }

    /// <summary>
    /// Добавить новую отметку выполнения для привычки.
    /// </summary>
    /// <param name="habitId">Идентификатор привычки.</param>
    /// <param name="request">Данные отметки (дата, значение, статус).</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Созданная отметка.</returns>
    /// <response code="201">Отметка успешно создана.</response>
    /// <response code="400">Некорректные данные запроса.</response>
    /// <response code="401">Пользователь не авторизован.</response>
    /// <response code="404">Привычка не найдена или не принадлежит пользователю.</response>
    [HttpPost]
    [ProducesResponseType(typeof(HabitEntryDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<HabitEntryDto>> AddEntry(
        Guid habitId,
        [FromBody] CreateHabitEntryDto request,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetCurrentUserId();
            var entry = await _habitEntryService.AddHabitEntryAsync(userId, habitId, request, cancellationToken);
            // Возвращаем 201 без Location, т.к. нет отдельного GET эндпоинта для одной отметки
            return Created(string.Empty, entry);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { error = "Habit not found or does not belong to user." });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        // Другие исключения обрабатываются глобальным фильтром
    }

    /// <summary>
    /// Вспомогательный метод для получения ID текущего пользователя из JWT.
    /// </summary>
    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userIdClaim is null)
            throw new UnauthorizedAccessException("User ID not found in token.");
        return Guid.Parse(userIdClaim);
    }
}
