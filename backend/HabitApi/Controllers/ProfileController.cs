using HabitApi.Models.DTO;
using HabitApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HabitApi.Controllers;

/// <summary>
/// Контроллер для управления профилем текущего пользователя.
/// Требует аутентификации.
/// </summary>
[ApiController]
[Route("api/profile")]
[Authorize]
public sealed class ProfileController : ControllerBase
{
    private readonly IProfileService _profileService;

    public ProfileController(IProfileService profileService)
    {
        _profileService = profileService;
    }

    /// <summary>
    /// Получить профиль текущего пользователя.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Профиль пользователя.</returns>
    /// <response code="200">Профиль успешно получен.</response>
    /// <response code="401">Пользователь не авторизован.</response>
    /// <response code="404">Профиль не найден.</response>
    [HttpGet]
    [ProducesResponseType(typeof(UserProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserProfileDto>> GetProfile(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var profile = await _profileService.GetProfileAsync(userId, cancellationToken);

        if (profile is null)
            return NotFound(new { error = "Profile not found." });

        return Ok(profile);
    }

    /// <summary>
    /// Обновить профиль текущего пользователя.
    /// </summary>
    /// <param name="request">Обновлённые данные профиля.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Обновлённый профиль.</returns>
    /// <response code="200">Профиль успешно обновлён.</response>
    /// <response code="400">Некорректные данные.</response>
    /// <response code="401">Пользователь не авторизован.</response>
    /// <response code="404">Профиль не найден.</response>
    [HttpPut]
    [ProducesResponseType(typeof(UserProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserProfileDto>> UpdateProfile(
        UpdateUserProfileDto request,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var profile = await _profileService.UpdateProfileAsync(userId, request, cancellationToken);

        if (profile is null)
            return NotFound(new { error = "Profile not found." });

        return Ok(profile);
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
