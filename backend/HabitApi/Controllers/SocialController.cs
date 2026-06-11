using System.Security.Claims;
using System.Text.Json;
using HabitApi.Data;
using HabitApi.Models.Domain;
using HabitApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HabitApi.Controllers;

[ApiController]
[Route("api/social")]
[Authorize]
public class SocialController : ControllerBase
{
    private readonly ISocialService _socialService;
    public SocialController(ISocialService socialService) => _socialService = socialService;

    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet("feed")]
    public async Task<ActionResult> GetFeed([FromQuery] string city) => Ok(await _socialService.GetCityFeedAsync(city));

    [HttpPost("feed")]
    public async Task<ActionResult> PostToFeed([FromBody] PostFeedRequest request)
    {
        var feed = await _socialService.PostToFeedAsync(request.City, request.HabitName);
        return Ok(feed);
    }

    [HttpPost("friends/{friendId}")]
    public async Task<ActionResult> SendFriendRequest(Guid friendId) => Ok(await _socialService.SendFriendRequestAsync(GetUserId(), friendId));

    [HttpPut("friends/{friendshipId}/status")]
    public async Task<ActionResult> AcceptFriendRequest(Guid friendshipId)
    {
        var result = await _socialService.AcceptFriendRequestAsync(GetUserId(), friendshipId);
        if (result == null) return NotFound("Friend request not found or already processed.");
        return Ok(result);
    }

    [HttpGet("friends")]
    public async Task<ActionResult> GetFriends() => Ok(await _socialService.GetFriendsAsync(GetUserId()));

    [HttpGet("challenges")]
    public async Task<ActionResult> GetChallenges() => Ok(await _socialService.GetChallengesAsync());

    [HttpPost("challenges")]
    public async Task<ActionResult> CreateChallenge([FromBody] CreateChallengeRequest request) =>
        Ok(await _socialService.CreateChallengeAsync(GetUserId(), request.Name, request.Description, request.StartDate, request.EndDate));

    [HttpPost("challenges/{challengeId}/join")]
    public async Task<ActionResult> JoinChallenge(Guid challengeId)
    {
        var result = await _socialService.JoinChallengeAsync(GetUserId(), challengeId);
        if (result == null) return NotFound("Challenge not found, inactive, or already joined.");
        return Ok(result);
    }
}

public record PostFeedRequest(string City, string HabitName);
public record CreateChallengeRequest(string Name, string Description, DateTime StartDate, DateTime EndDate);
