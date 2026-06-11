using System.Security.Claims;
using HabitApi.Services;
using HabitApi.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace HabitApi.Controllers;

[ApiController]
[Route("api/streaks")]
public class StreaksController : ControllerBase
{
    private readonly IStreakService _streakService;

    public StreaksController(IStreakService streakService) => _streakService = streakService;

    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<ActionResult<List<StreakDto>>> GetStreaks()
    {
        var streaks = await _streakService.GetUserStreaksAsync(GetUserId());
        return Ok(streaks);
    }

    [HttpGet("{habitId}")]
    public async Task<ActionResult<StreakDto>> GetHabitStreak(Guid habitId)
    {
        var streak = await _streakService.GetHabitStreakAsync(GetUserId(), habitId);
        if (streak == null) return NotFound();
        return Ok(streak);
    }
}
