using HabitApi.Data;
using HabitApi.Exceptions;
using HabitApi.Models.Domain;
using HabitApi.Models.DTO;
using HabitApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HabitApi.Services;

/// <summary>
/// Сервис аутентификации: регистрация, вход и выпуск JWT-токенов.
/// </summary>
public sealed class AuthService : IAuthService
{
    private readonly AppDbContext _dbContext;
    private readonly string _jwtSecret;
    private readonly string _jwtIssuer;
    private readonly string _jwtAudience;

    public AuthService(AppDbContext dbContext, IConfiguration configuration)
    {
        _dbContext = dbContext;
        _jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET")
            ?? configuration["Jwt:Secret"]
            ?? throw new InvalidOperationException("JWT_SECRET is not configured.");
        _jwtIssuer = configuration["Jwt:Issuer"] ?? "HabitApi";
        _jwtAudience = configuration["Jwt:Audience"] ?? "HabitApiClient";
    }

    /// <summary>
    /// Регистрирует нового пользователя и сразу возвращает токен доступа.
    /// </summary>
    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken)
    {
        // Нормализуем email, чтобы не плодить дубли из-за регистра и пробелов.
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var normalizedCity = request.City.Trim();

        var existing = await _dbContext.Users
            .FirstOrDefaultAsync(x => x.Email.ToLower() == normalizedEmail, cancellationToken);
        if (existing is not null)
            throw new ConflictException("User with this email already exists.");

        // Создаем доменную сущность пользователя с хешем пароля.
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = normalizedEmail,
            City = normalizedCity,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            CreatedAtUtc = DateTime.UtcNow
        };

        // Генерируем ответ заранее, чтобы ошибка в JWT не случилась уже после сохранения пользователя.
        var authResponse = BuildAuthResponse(user);

        _dbContext.Users.Add(user);

        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        // Ловим конфликт по уникальному email даже если он проявился на уровне БД.
        catch (DbUpdateException ex) when (IsUniqueEmailViolation(ex))
        {
            throw new ConflictException("User with this email already exists.");
        }

        return authResponse;
    }

    /// <summary>
    /// Проверяет учетные данные пользователя и возвращает JWT при успешном входе.
    /// </summary>
    public async Task<AuthResponseDto?> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        var user = await _dbContext.Users
            .FirstOrDefaultAsync(x => x.Email.ToLower() == normalizedEmail, cancellationToken);
        if (user is null)
            return null;

        var isValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
        if (!isValid)
            return null;

        return BuildAuthResponse(user);
    }

    /// <summary>
    /// Собирает DTO ответа авторизации и подписывает access token.
    /// </summary>
    private AuthResponseDto BuildAuthResponse(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtSecret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature),
            Issuer = _jwtIssuer,
            Audience = _jwtAudience
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var accessToken = tokenHandler.WriteToken(token);

        return new AuthResponseDto
        {
            UserId = user.Id,
            Email = user.Email,
            AccessToken = accessToken
        };
    }

    /// <summary>
    /// Проверяет, что ошибка сохранения связана именно с уникальностью email.
    /// </summary>
    private static bool IsUniqueEmailViolation(DbUpdateException exception)
    {
        var message = exception.InnerException?.Message ?? exception.Message;

        return message.Contains("IX_Users_Email", StringComparison.OrdinalIgnoreCase)
            || message.Contains("duplicate key", StringComparison.OrdinalIgnoreCase);
    }
}
