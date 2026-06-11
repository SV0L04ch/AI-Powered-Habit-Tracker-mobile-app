using System.Security.Claims;
using HabitApi.Services;
using HabitApi.Models.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HabitApi.Controllers;

[ApiController]
[Route("api/locations")]
[Authorize]
public class LocationsController : ControllerBase
{
    private readonly ILocationService _locationService;

    public LocationsController(ILocationService locationService) => _locationService = locationService;

    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet("entries/{habitEntryId}")]
    public async Task<ActionResult<List<HabitLocation>>> GetLocations(Guid habitEntryId) =>
        Ok(await _locationService.GetLocationsByEntryAsync(GetUserId(), habitEntryId));

    [HttpPost("entries/{habitEntryId}")]
    public async Task<ActionResult<HabitLocation>> AddLocation(Guid habitEntryId, [FromBody] AddLocationRequest request)
    {
        var location = await _locationService.AddLocationAsync(GetUserId(), habitEntryId, request.Latitude, request.Longitude, request.Name);
        return Ok(location);
    }

    [HttpDelete("{locationId}")]
    public async Task<ActionResult> DeleteLocation(Guid locationId)
    {
        var result = await _locationService.DeleteLocationAsync(GetUserId(), locationId);
        if (!result) return NotFound("Location not found.");
        return NoContent();
    }
}

public record AddLocationRequest(double Latitude, double Longitude, string? Name);
