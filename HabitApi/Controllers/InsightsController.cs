using HabitApi.Models.DTO;
using HabitApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HabitApi.Controllers;

/// <summary>
/// Контроллер для генерации ИИ-подсказок и поддержки привычек.
/// Требует аутентификации.
/// </summary>
[ApiController]
[Route("api/habits/{habitId:guid}/insights")]
[Authorize]
public sealed class InsightsController : ControllerBase
{
    private readonly IHabitService _habitService;
    private readonly IAiInsightsService _aiInsightsService;

    public InsightsController(IHabitService habitService, IAiInsightsService aiInsightsService)
    {
        _habitService = habitService;
        _aiInsightsService = aiInsightsService;
    }

    /// <summary>
    /// Генерирует поддерживающее сообщение для привычки в зависимости от сценария.
    /// </summary>
    /// <param name="habitId">Идентификатор привычки.</param>
    /// <param name="request">Сценарий запроса (например, "lazy", "relapse", "skip").</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Сообщение поддержки от ИИ.</returns>
    /// <response code="200">Сообщение успешно сгенерировано.</response>
    /// <response code="400">Некорректный сценарий.</response>
    /// <response code="401">Пользователь не авторизован.</response>
    /// <response code="404">Привычка не найдена или не принадлежит пользователю.</response>
    [HttpPost("support")]
    [ProducesResponseType(typeof(HabitSupportResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<HabitSupportResponseDto>> BuildSupportMessage(
        Guid habitId,
        HabitSupportRequestDto request,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var habit = await _habitService.GetHabitByIdAsync(userId, habitId, cancellationToken);
        if (habit is null)
            return NotFound(new { error = "Habit not found." });

        try
        {
            var message = await _aiInsightsService.BuildHabitSupportMessageAsync(
                habit.Name,
                request.Scenario,
                cancellationToken);

            return Ok(new HabitSupportResponseDto
            {
                HabitId = habitId,
                Scenario = request.Scenario,
                Message = message
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        // Остальные исключения обрабатываются глобальным фильтром
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
