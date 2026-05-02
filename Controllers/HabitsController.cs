using HabitApi.Models.DTO;
using HabitApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HabitApi.Controllers;

/// <summary>
/// Контроллер для управления привычками пользователя.
/// </summary>
[ApiController]
[Route("api/habits")]
[Authorize] // Требуем аутентификацию
public sealed class HabitsController : ControllerBase
{
    private readonly IHabitService _habitService;

    public HabitsController(IHabitService habitService)
    {
        _habitService = habitService;
    }

    /// <summary>
    /// Получить все привычки текущего пользователя.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Коллекция привычек.</returns>
    /// <response code="200">Успешное получение.</response>
    /// <response code="401">Пользователь не авторизован.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<HabitDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IReadOnlyCollection<HabitDto>>> GetUserHabits(
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var habits = await _habitService.GetUserHabitsAsync(userId, cancellationToken);
        return Ok(habits);
    }

    /// <summary>
    /// Получить конкретную привычку по идентификатору.
    /// </summary>
    /// <param name="habitId">Идентификатор привычки.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Привычка.</returns>
    /// <response code="200">Успешное получение.</response>
    /// <response code="401">Пользователь не авторизован.</response>
    /// <response code="404">Привычка не найдена или не принадлежит пользователю.</response>
    [HttpGet("{habitId:guid}")]
    [ProducesResponseType(typeof(HabitDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<HabitDto>> GetHabitById(
        Guid habitId,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var habit = await _habitService.GetHabitByIdAsync(userId, habitId, cancellationToken);
        if (habit is null)
            return NotFound(new { error = "Habit not found." });
        return Ok(habit);
    }

    /// <summary>
    /// Создать новую привычку.
    /// </summary>
    /// <param name="request">Данные для создания привычки.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Созданная привычка.</returns>
    /// <response code="201">Привычка успешно создана.</response>
    /// <response code="400">Некорректные данные.</response>
    /// <response code="401">Пользователь не авторизован.</response>
    [HttpPost]
    [ProducesResponseType(typeof(HabitDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<HabitDto>> CreateHabit(
        [FromBody] CreateHabitDto request,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var habit = await _habitService.CreateHabitAsync(userId, request, cancellationToken);
        return CreatedAtAction(nameof(GetHabitById), new { habitId = habit.Id }, habit);
    }

    /// <summary>
    /// Обновить существующую привычку.
    /// </summary>
    /// <param name="habitId">Идентификатор привычки.</param>
    /// <param name="request">Обновлённые данные.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Обновлённая привычка.</returns>
    /// <response code="200">Успешное обновление.</response>
    /// <response code="400">Некорректные данные.</response>
    /// <response code="401">Пользователь не авторизован.</response>
    /// <response code="404">Привычка не найдена или не принадлежит пользователю.</response>
    [HttpPut("{habitId:guid}")]
    [ProducesResponseType(typeof(HabitDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<HabitDto>> UpdateHabit(
        Guid habitId,
        [FromBody] UpdateHabitDto request,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var habit = await _habitService.UpdateHabitAsync(userId, habitId, request, cancellationToken);
        if (habit is null)
            return NotFound(new { error = "Habit not found." });
        return Ok(habit);
    }

    /// <summary>
    /// Удалить привычку (мягкое или жёсткое удаление – зависит от сервиса).
    /// </summary>
    /// <param name="habitId">Идентификатор привычки.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Статус 204 при успехе, иначе 404.</returns>
    /// <response code="204">Удаление успешно.</response>
    /// <response code="401">Пользователь не авторизован.</response>
    /// <response code="404">Привычка не найдена или не принадлежит пользователю.</response>
    [HttpDelete("{habitId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteHabit(
        Guid habitId,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var deleted = await _habitService.DeleteHabitAsync(userId, habitId, cancellationToken);
        if (!deleted)
            return NotFound(new { error = "Habit not found." });
        return NoContent();
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
