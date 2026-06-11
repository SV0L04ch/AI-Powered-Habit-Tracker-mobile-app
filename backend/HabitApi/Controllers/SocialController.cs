using System.Security.Claims;
using System.Text.Json;
using HabitApi.Data;
using HabitApi.Models.Domain;
using HabitApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace HabitApi.Controllers;

[ApiController]
[Route("api/social")]
public class SocialController : ControllerBase
{
    private readonly ISocialService _socialService;
    public SocialController(ISocialService socialService) => _socialService = socialService;

    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet("feed")]
    public async Task<ActionResult> GetFeed([FromQuery] string city) => Ok(await _socialService.GetCityFeedAsync(city));

    [HttpPost("friends/{friendId}")]
    public async Task<ActionResult> SendFriendRequest(Guid friendId) => Ok(await _socialService.SendFriendRequestAsync(GetUserId(), friendId));

    [HttpGet("friends")]
    public async Task<ActionResult> GetFriends() => Ok(await _socialService.GetFriendsAsync(GetUserId()));

    [HttpGet("challenges")]
    public async Task<ActionResult> GetChallenges() => Ok(await _socialService.GetChallengesAsync());

    [HttpPost("challenges")]
    public async Task<ActionResult> CreateChallenge([FromBody] CreateChallengeRequest request) =>
        Ok(await _socialService.CreateChallengeAsync(GetUserId(), request.Name, request.Description, request.StartDate, request.EndDate));
}

public record CreateChallengeRequest(string Name, string Description, DateTime StartDate, DateTime EndDate);
