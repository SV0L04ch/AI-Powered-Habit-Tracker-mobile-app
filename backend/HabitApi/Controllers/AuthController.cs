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
    private readonly IConfiguration _configuration;

    public AuthController(IAuthService authService, IConfiguration configuration)
    {
        _authService = authService;
        _configuration = configuration;
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
        var frontendBaseUrl = _configuration["FrontendBaseUrl"] ?? "http://localhost:5173";
        var loginUrl = $"{frontendBaseUrl.TrimEnd('/')}/login";

        if (user is null)
        {
            return Content(
                BuildConfirmationPage(
                    "Ссылка недействительна",
                    "Ссылка подтверждения устарела или уже была использована.",
                    loginUrl,
                    false),
                "text/html; charset=utf-8");
        }

        return Content(
            BuildConfirmationPage(
                "Email подтвержден",
                "Аккаунт готов. Сейчас перенаправим вас на страницу входа.",
                loginUrl,
                true),
            "text/html; charset=utf-8");
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

    private static string BuildConfirmationPage(string title, string message, string loginUrl, bool success)
    {
        var accent = success ? "#16a34a" : "#ef4444";
        return $$"""
<!doctype html>
<html lang="ru">
<head>
  <meta charset="utf-8">
  <meta name="viewport" content="width=device-width, initial-scale=1">
  <meta http-equiv="refresh" content="3; url={{loginUrl}}">
  <title>{{title}}</title>
  <style>
    :root { color-scheme: light; font-family: Inter, system-ui, -apple-system, BlinkMacSystemFont, 'Segoe UI', sans-serif; }
    body { min-height: 100vh; margin: 0; display: grid; place-items: center; background: radial-gradient(circle at 20% 0%, rgba(66, 211, 146, .18), transparent 34vw), #f8fafc; color: #0b1020; }
    main { width: min( calc(100% - 32px), 460px ); padding: 28px; border: 1px solid rgba(15, 23, 42, .1); border-radius: 28px; background: rgba(255,255,255,.88); box-shadow: 0 18px 45px rgba(15, 23, 42, .08); }
    .mark { width: 58px; height: 58px; border-radius: 20px; display: grid; place-items: center; color: white; background: {{accent}}; font-weight: 900; font-size: 28px; }
    h1 { margin: 20px 0 8px; font-size: 32px; line-height: 1.05; }
    p { margin: 0 0 22px; color: #667085; font-weight: 600; line-height: 1.45; }
    a { min-height: 48px; padding: 0 18px; border-radius: 16px; display: inline-flex; align-items: center; justify-content: center; color: white; background: linear-gradient(135deg, #0b1020, #2563eb 58%, #42d392); text-decoration: none; font-weight: 800; }
  </style>
</head>
<body>
  <main>
    <div class="mark">{{(success ? "✓" : "!")}}</div>
    <h1>{{title}}</h1>
    <p>{{message}}</p>
    <a href="{{loginUrl}}">Перейти ко входу</a>
  </main>
  <script>setTimeout(() => { window.location.href = "{{loginUrl}}"; }, 3000);</script>
</body>
</html>
""";
    }
}
