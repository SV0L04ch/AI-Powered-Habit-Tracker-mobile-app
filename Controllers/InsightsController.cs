using System.Security.Claims;
using HabitApi.Models.DTO;
using HabitApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace HabitApi.Controllers;

[ApiController]
[Route("api/habits/{habitId:guid}/insights")]
[Authorize]
public sealed class InsightsController : ControllerBase
{
    private readonly IHabitService _habitService;
    private readonly IAiInsightsService _aiInsightsService;
    private readonly IStatsService _statsService;

    public InsightsController(
        IHabitService habitService,
        IAiInsightsService aiInsightsService,
        IStatsService statsService)
    {
        _habitService = habitService;
        _aiInsightsService = aiInsightsService;
        _statsService = statsService;
    }

    [HttpPost("support")]
    [ProducesResponseType(typeof(HabitSupportResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<HabitSupportResponseDto>> BuildSupportMessage(
        Guid habitId,
        [FromBody] HabitSupportRequestDto request,
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
    }

    [HttpPost("weather-summary")]
    [ProducesResponseType(typeof(HabitWeatherInsightResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<HabitWeatherInsightResponseDto>> BuildWeatherSummary(
        Guid habitId,
        [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] HabitWeatherInsightRequestDto? request,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        request ??= new HabitWeatherInsightRequestDto();
        var targetDate = request.Date ?? DateOnly.FromDateTime(DateTime.UtcNow);

        try
        {
            var summary = await _statsService.GetHabitWeatherInsightAsync(
                userId,
                habitId,
                targetDate,
                request.IncludePreviousDayComparison,
                cancellationToken);

            return Ok(summary);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { error = "Habit not found." });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userIdClaim is null)
            throw new UnauthorizedAccessException("User ID not found in token.");

        return Guid.Parse(userIdClaim);
    }
}
