using HabitApi.Models.DTO;
using HabitApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HabitApi.Controllers;

/// <summary>
/// Контроллер аутентификации: регистрация, подтверждение email, вход и выход.
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
    /// После успешной регистрации отправляется письмо для подтверждения email.
    /// JWT-токен не выдаётся до подтверждения.
    /// </summary>
    /// <param name="request">Данные для регистрации (email, пароль, город).</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Ответ 201 с данными пользователя и сообщением.</returns>
    /// <response code="201">Пользователь успешно зарегистрирован.</response>
    /// <response code="400">Ошибка валидации входных данных.</response>
    /// <response code="409">Пользователь с таким email уже существует.</response>
    [HttpPost("register")]
    [ProducesResponseType(typeof(RegistrationResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register(RegisterRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _authService.RegisterAsync(request, cancellationToken);
        return Created(string.Empty, result);
    }

    /// <summary>
    /// Подтверждение email пользователя по ссылке из письма.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя.</param>
    /// <param name="token">Токен подтверждения.</param>
    /// <returns>Сообщение об успехе или ошибка, если ссылка недействительна.</returns>
    /// <response code="200">Email подтверждён.</response>
    /// <response code="400">Неверный или истёкший токен подтверждения.</response>
    [HttpGet("confirm-email")]
    public async Task<IActionResult> ConfirmEmail(Guid userId, string token)
    {
        var user = await _authService.ConfirmEmailAsync(userId, token);
        if (user is null)
            return BadRequest(new { error = "Invalid or expired confirmation link." });

        return Ok(new { message = "Email confirmed. You can now log in." });
    }

    /// <summary>
    /// Вход пользователя. При успешном входе устанавливает JWT-токен в куки.
    /// </summary>
    /// <param name="request">Учётные данные (email, пароль).</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Ответ с токеном доступа и данными пользователя.</returns>
    /// <response code="200">Вход выполнен успешно.</response>
    /// <response code="401">Неверный email или пароль, либо email не подтверждён.</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(LoginRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _authService.LoginAsync(request, cancellationToken);
        if (result?.AccessToken is null)
            return Unauthorized(new { error = "Invalid email or password." });

        SetAccessTokenCookie(result.AccessToken);
        return Ok(result);
    }

    /// <summary>
    /// Выход пользователя: удаляет куки с токеном доступа.
    /// </summary>
    /// <returns>Статус 204 без содержимого.</returns>
    /// <response code="204">Выход выполнен.</response>
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("access_token");
        return NoContent();
    }

    /// <summary>
    /// Устанавливает JWT-токен в HttpOnly-куку для последующих запросов.
    /// </summary>
    private void SetAccessTokenCookie(string token)
    {
        var isDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = !isDevelopment,                        // true в production, false в development
            SameSite = SameSiteMode.Strict,                 // защита от CSRF
            Expires = DateTimeOffset.UtcNow.AddHours(1)
        };

        Response.Cookies.Append("access_token", token, cookieOptions);
    }
}
