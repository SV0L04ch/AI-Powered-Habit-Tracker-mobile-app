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

public sealed class AuthService : IAuthService
{
    private readonly AppDbContext _dbContext;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;
    private readonly string _jwtSecret;
    private readonly string _jwtIssuer;
    private readonly string _jwtAudience;

    public AuthService(AppDbContext dbContext, IEmailService emailService, IConfiguration configuration)
    {
        _dbContext = dbContext;
        _emailService = emailService;
        _configuration = configuration;
        _jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET")
            ?? configuration["Jwt:Secret"]
            ?? throw new InvalidOperationException("JWT_SECRET is not configured.");
        _jwtIssuer = configuration["Jwt:Issuer"] ?? "HabitApi";
        _jwtAudience = configuration["Jwt:Audience"] ?? "HabitApiClient";
    }

    public async Task<RegistrationResponseDto> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var normalizedCity = request.City.Trim();

        var existing = await _dbContext.Users
            .FirstOrDefaultAsync(x => x.Email.ToLower() == normalizedEmail, cancellationToken);
        if (existing is not null)
            throw new ConflictException("User with this email already exists.");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = normalizedEmail,
            City = normalizedCity,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            CreatedAtUtc = DateTime.UtcNow,
            IsEmailConfirmed = false,
            EmailConfirmationToken = Guid.NewGuid().ToString()
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var baseUrl = _configuration["AppBaseUrl"] ?? "http://localhost:5093";
        var confirmationLink = $"{baseUrl}/api/auth/confirm-email?userId={user.Id}&token={user.EmailConfirmationToken}";
        _ = Task.Run(() => _emailService.SendConfirmationEmailAsync(user.Email, confirmationLink), cancellationToken);

        return new RegistrationResponseDto
        {
            UserId = user.Id,
            Email = user.Email,
            Message = "Registration successful. Please check your email to confirm your account."
        };
    }

    public async Task<User?> ConfirmEmailAsync(Guid userId, string token)
    {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == userId && u.EmailConfirmationToken == token);
        if (user == null || user.IsEmailConfirmed)
            return null;

        user.IsEmailConfirmed = true;
        user.EmailConfirmationToken = null;
        await _dbContext.SaveChangesAsync();
        return user;
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        var user = await _dbContext.Users
            .FirstOrDefaultAsync(x => x.Email.ToLower() == normalizedEmail, cancellationToken);
        if (user is null)
            return null;

        if (!user.IsEmailConfirmed)
            throw new UnauthorizedAccessException("Email not confirmed. Please check your inbox.");

        var isValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
        if (!isValid)
            return null;

        return BuildAuthResponse(user);
    }

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
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
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

    private static bool IsUniqueEmailViolation(DbUpdateException exception)
    {
        var message = exception.InnerException?.Message ?? exception.Message;
        return message.Contains("IX_Users_Email", StringComparison.OrdinalIgnoreCase)
            || message.Contains("duplicate key", StringComparison.OrdinalIgnoreCase);
    }
}
