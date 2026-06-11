using System.Security.Claims;
using HabitApi.Services;
using HabitApi.Models.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HabitApi.Controllers;

[ApiController]
[Route("api/leagues")]
[Authorize]
public class LeaguesController : ControllerBase
{
    private readonly ILeagueService _leagueService;

    public LeaguesController(ILeagueService leagueService) => _leagueService = leagueService;

    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<ActionResult<List<League>>> GetLeagues() => Ok(await _leagueService.GetLeaguesAsync());

    [HttpGet("mine")]
    public async Task<ActionResult<League>> GetUserLeague()
    {
        var league = await _leagueService.GetUserLeagueAsync(GetUserId());
        if (league == null) return NotFound("No league found for your XP level.");
        return Ok(league);
    }
}
