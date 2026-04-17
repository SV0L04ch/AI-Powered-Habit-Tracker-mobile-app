using HabitApi.Models.DTO;
using HabitApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HabitApi.Controllers;

/// <summary>
/// Контроллер для аутентификации пользователей (регистрация и вход).
/// </summary>
[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Регистрация нового пользователя.
    /// </summary>
    /// <param name="request">Данные для регистрации (email, пароль, имя).</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Данные авторизованного пользователя (токен, userId).</returns>
    /// <response code="201">Пользователь успешно создан.</response>
    /// <response code="400">Некорректные данные запроса.</response>
    /// <response code="409">Пользователь с таким email уже существует.</response>
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AuthResponseDto>> Register(
        [FromBody] RegisterRequestDto request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _authService.RegisterAsync(request, cancellationToken);
            // Возвращаем 201 без Location, так как ресурс пользователя не имеет отдельного GET эндпоинта.
            // Альтернатива: return CreatedAtAction(nameof(UsersController.GetById), "Users", new { id = result.UserId }, result);
            return Created(string.Empty, result);
        }
        catch (InvalidOperationException ex)
        {
            // Конфликт: пользователь уже существует
            return Conflict(new { error = ex.Message });
        }
        // Другие исключения обрабатываются глобальным фильтром
    }

    /// <summary>
    /// Вход пользователя в систему.
    /// </summary>
    /// <param name="request">Учётные данные (email, пароль).</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Данные авторизованного пользователя (токен, userId).</returns>
    /// <response code="200">Успешный вход.</response>
    /// <response code="401">Неверный email или пароль.</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponseDto>> Login(
        [FromBody] LoginRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _authService.LoginAsync(request, cancellationToken);
        if (result is null)
        {
            return Unauthorized(new { error = "Invalid email or password." });
        }
        return Ok(result);
    }
}
