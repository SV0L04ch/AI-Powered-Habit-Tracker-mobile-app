using System.Security.Claims;
using HabitApi.Services;
using HabitApi.Models.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HabitApi.Controllers;

[ApiController]
[Route("api/photos")]
[Authorize]
public class PhotosController : ControllerBase
{
    private readonly IPhotoService _photoService;

    public PhotosController(IPhotoService photoService) => _photoService = photoService;

    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet("entries/{habitEntryId}")]
    public async Task<ActionResult<List<HabitPhoto>>> GetPhotos(Guid habitEntryId) =>
        Ok(await _photoService.GetPhotosByEntryAsync(GetUserId(), habitEntryId));

    [HttpPost("entries/{habitEntryId}")]
    public async Task<ActionResult<HabitPhoto>> AddPhoto(Guid habitEntryId, [FromBody] AddPhotoRequest request)
    {
        var photo = await _photoService.AddPhotoAsync(GetUserId(), habitEntryId, request.PhotoUrl, request.Caption);
        return Ok(photo);
    }

    [HttpDelete("{photoId}")]
    public async Task<ActionResult> DeletePhoto(Guid photoId)
    {
        var result = await _photoService.DeletePhotoAsync(GetUserId(), photoId);
        if (!result) return NotFound("Photo not found.");
        return NoContent();
    }
}

public record AddPhotoRequest(string PhotoUrl, string? Caption);
