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
    /// <param name="request">Данные для регистрации (email, пароль, город).</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Данные пользователя (userId, email) без токена; сам токен передаётся в HttpOnly cookie.</returns>
    /// <response code="201">Пользователь успешно создан.</response>
    /// <response code="400">Некорректные данные запроса.</response>
    /// <response code="409">Пользователь с таким email уже существует.</response>
    [HttpPost("register")]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _authService.RegisterAsync(request, cancellationToken);
        
        // Устанавливаем HttpOnly cookie с access token
        SetAccessTokenCookie(result.AccessToken);
        
        // Возвращаем данные без чувствительного токена
        return Created(string.Empty, new { result.UserId, result.Email });
    }

    /// <summary>
    /// Вход пользователя в систему.
    /// </summary>
    /// <param name="request">Учётные данные (email, пароль).</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Данные пользователя (userId, email) без токена; токен передаётся в HttpOnly cookie.</returns>
    /// <response code="200">Успешный вход.</response>
    /// <response code="401">Неверный email или пароль.</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _authService.LoginAsync(request, cancellationToken);
        if (result is null)
            return Unauthorized(new { error = "Invalid email or password." });
        
        SetAccessTokenCookie(result.AccessToken);
        
        return Ok(new { result.UserId, result.Email });
    }

    /// <summary>
    /// Выход пользователя – удаляет cookie с токеном.
    /// </summary>
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("access_token");
        return NoContent();
    }

    /// <summary>
    /// Устанавливает HttpOnly cookie с access token.
    /// </summary>
    private void SetAccessTokenCookie(string token)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,                      // Недоступен для JavaScript
            Secure = !Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")?.Equals("Development") ?? false, // В продакшене true
            SameSite = SameSiteMode.Lax,          // Для кросс-доменных запросов в разработке можно использовать None, но тогда нужен Secure=true
            Expires = DateTimeOffset.UtcNow.AddHours(1)   // Должно совпадать с Expiration в JWT
        };
        
        // В локальной разработке (без HTTPS) можно временно установить Secure=false
        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
        {
            cookieOptions.Secure = false;
        }
        
        Response.Cookies.Append("access_token", token, cookieOptions);
    }
}
