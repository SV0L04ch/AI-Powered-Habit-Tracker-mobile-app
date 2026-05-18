using DotNetEnv;
using HabitApi.Data;
using HabitApi.Exceptions;
using HabitApi.Models.Domain;
using HabitApi.Services;
using HabitApi.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;
using Polly;
using Polly.Extensions.Http;
using Microsoft.AspNetCore.RateLimiting;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Structured logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Настройка подключения к PostgreSQL
var dbHost = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";
var dbPort = Environment.GetEnvironmentVariable("DB_PORT") ?? "5432";
var dbName = Environment.GetEnvironmentVariable("DB_NAME") ?? "habit_tracker";
var dbUser = Environment.GetEnvironmentVariable("DB_USER") ?? "habit_user";
var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "default_password";
var connectionString = $"Host={dbHost};Port={dbPort};Database={dbName};Username={dbUser};Password={dbPassword}";

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// Identity
builder.Services.AddIdentityCore<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedEmail = true;
    options.User.RequireUniqueEmail = true;
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
    .AddRoles<IdentityRole<Guid>>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders()
    .AddSignInManager();

// JWT конфигурация
var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET")
    ?? builder.Configuration["Jwt:Secret"]
    ?? throw new InvalidOperationException("JWT_SECRET is not configured.");
if (string.IsNullOrWhiteSpace(jwtSecret) || jwtSecret.Length < 32)
    throw new InvalidOperationException("JWT secret must be at least 32 characters long.");
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "HabitApi";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "HabitApiClient";
var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));

// CORS
var corsOrigins = builder.Configuration.GetSection("CorsOrigins").Get<string[]>()
    ?? new[] { "http://localhost:3000", "http://localhost:19006", "http://localhost:5093" };
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(corsOrigins)
              .AllowCredentials()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Аутентификация через JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = signingKey
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var token = context.Request.Cookies["access_token"];
                if (!string.IsNullOrEmpty(token))
                    context.Token = token;
                return Task.CompletedTask;
            },
            OnChallenge = async context =>
            {
                context.HandleResponse();
                if (context.Response.HasStarted) return;
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/problem+json";
                await context.Response.WriteAsJsonAsync(new ProblemDetails
                {
                    Status = StatusCodes.Status401Unauthorized,
                    Title = "Unauthorized",
                    Detail = "A valid access token is required to access this resource.",
                    Type = "https://httpstatuses.com/401",
                    Instance = context.Request.Path
                });
            },
            OnForbidden = async context =>
            {
                if (context.Response.HasStarted) return;
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                context.Response.ContentType = "application/problem+json";
                await context.Response.WriteAsJsonAsync(new ProblemDetails
                {
                    Status = StatusCodes.Status403Forbidden,
                    Title = "Forbidden",
                    Detail = "You do not have permission to access this resource.",
                    Type = "https://httpstatuses.com/403",
                    Instance = context.Request.Path
                });
            }
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddProblemDetails();

// HttpClient для WeatherService с Polly (timeout 10s + 3 retry)
builder.Services
    .AddHttpClient<IWeatherService, WeatherService>(client =>
    {
        client.Timeout = TimeSpan.FromSeconds(10);
    })
    .AddPolicyHandler(HttpPolicyExtensions
        .HandleTransientHttpError()
        .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))));

// HttpClient для AiInsightsService (таймаут 5 минут для медленных локальных моделей)
builder.Services
    .AddHttpClient<IAiInsightsService, AiInsightsService>(client =>
    {
        client.Timeout = TimeSpan.FromMinutes(5);
    });

// Swagger
builder.Services.AddSwaggerGen();

// Redis
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis")
                            ?? Environment.GetEnvironmentVariable("REDIS_CONNECTION")
                            ?? "localhost:6379";
    options.InstanceName = "HabitTracker_";
});

// Rate Limiter
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("auth", config =>
    {
        config.PermitLimit = 5;
        config.Window = TimeSpan.FromMinutes(1);
        config.QueueLimit = 0;
    });
});

// Регистрация остальных сервисов
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IHabitService, HabitService>();
builder.Services.AddScoped<IHabitEntryService, HabitEntryService>();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<IStatsService, StatsService>();
builder.Services.AddScoped<IEmailService, EmailService>();
// IWeatherService и IAiInsightsService уже зарегистрированы через AddHttpClient

builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

var app = builder.Build();

// Глобальный обработчик исключений
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
        var problemDetails = CreateProblemDetails(context, exception, app.Environment.IsDevelopment());
        context.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/problem+json";
        await context.Response.WriteAsJsonAsync(problemDetails);
    });
});

// Swagger UI только в режиме разработки
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();
app.MapControllers();

app.MapGet("/health", () => Results.Ok("Healthy"));

app.Run();

static ProblemDetails CreateProblemDetails(HttpContext context, Exception? exception, bool includeExceptionDetails)
{
    var statusCode = exception switch
    {
        ConflictException => StatusCodes.Status409Conflict,
        ArgumentException => StatusCodes.Status400BadRequest,
        UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
        KeyNotFoundException => StatusCodes.Status404NotFound,
        DbUpdateException => StatusCodes.Status400BadRequest,
        _ => StatusCodes.Status500InternalServerError
    };

    var title = statusCode switch
    {
        StatusCodes.Status400BadRequest => "Bad Request",
        StatusCodes.Status401Unauthorized => "Unauthorized",
        StatusCodes.Status404NotFound => "Not Found",
        StatusCodes.Status409Conflict => "Conflict",
        _ => "Internal Server Error"
    };

    var detail = statusCode == StatusCodes.Status500InternalServerError && !includeExceptionDetails
        ? "An unexpected error occurred."
        : exception?.Message;

    var problemDetails = new ProblemDetails
    {
        Status = statusCode,
        Title = title,
        Detail = detail,
        Type = $"https://httpstatuses.com/{statusCode}",
        Instance = context.Request.Path
    };
    problemDetails.Extensions["traceId"] = context.TraceIdentifier;
    return problemDetails;
}
