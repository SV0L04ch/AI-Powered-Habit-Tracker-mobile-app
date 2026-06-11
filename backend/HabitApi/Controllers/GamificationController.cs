using System.Security.Claims;
using HabitApi.Services;
using HabitApi.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace HabitApi.Controllers;

[ApiController]
[Route("api/gamification")]
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
}
