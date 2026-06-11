using System.Security.Claims;
using HabitApi.Services;
using HabitApi.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HabitApi.Controllers;

[ApiController]
[Route("api/gamification")]
[Authorize]
public class GamificationController : ControllerBase
{
    private readonly IGamificationService _gamificationService;

    public GamificationController(IGamificationService gamificationService) => _gamificationService = gamificationService;

    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<ActionResult<GamificationDto>> GetGamification()
    {
        var result = await _gamificationService.GetGamificationAsync(GetUserId());
        return Ok(result);
    }

    [HttpPost("xp")]
    public async Task<ActionResult> AddXP([FromBody] AddXPRequest request)
    {
        await _gamificationService.AddXPAsync(GetUserId(), request.Amount, request.Reason);
        return Ok(new { message = $"Added {request.Amount} XP." });
    }

    [HttpPost("check-achievements")]
    public async Task<ActionResult> CheckAchievements()
    {
        var result = await _gamificationService.CheckAndGrantAchievementsAsync(GetUserId());
        if (result == null) return Ok(new { message = "No new achievements." });
        return Ok(result);
    }
}

public record AddXPRequest(int Amount, string Reason);
