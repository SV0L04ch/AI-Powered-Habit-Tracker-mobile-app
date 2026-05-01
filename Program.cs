using DotNetEnv;
using HabitApi.Data;
using HabitApi.Exceptions;
using HabitApi.Services;
using HabitApi.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Настройка подключения к PostgreSQL
var host = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";
var port = Environment.GetEnvironmentVariable("DB_PORT") ?? "5432";
var database = Environment.GetEnvironmentVariable("DB_NAME") ?? "habit_tracker";
var username = Environment.GetEnvironmentVariable("DB_USER") ?? "habit_user";
var password = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "default_password";
var connectionString = $"Host={host};Port={port};Database={database};Username={username};Password={password}";

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// JWT конфигурация
var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET")
    ?? builder.Configuration["Jwt:Secret"]
    ?? throw new InvalidOperationException("JWT_SECRET is not configured.");
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "HabitApi";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "HabitApiClient";
var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));

// ----- НАСТРОЙКА CORS ДЛЯ КУК -----
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
                "http://localhost:3000",     // React Native Metro (обычно)
                "http://localhost:19006",    // Expo Web
                "http://localhost:5093")     // Ваш API (если нужно)
              .AllowCredentials()            // ОБЯЗАТЕЛЬНО для кук
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

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

        // ----- ЧТЕНИЕ ТОКЕНА ИЗ КУКИ -----
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
builder.Services.AddHttpClient();

// Регистрация сервисов
builder.Services.AddScoped<IWeatherService, WeatherService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IHabitService, HabitService>();
builder.Services.AddScoped<IHabitEntryService, HabitEntryService>();
builder.Services.AddScoped<IStatsService, StatsService>();
builder.Services.AddScoped<IAiInsightsService, AiInsightsService>();

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

app.UseCors("AllowFrontend");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

static ProblemDetails CreateProblemDetails(HttpContext context, Exception? exception, bool includeExceptionDetails)
{
    var statusCode = exception switch
    {
        ConflictException => StatusCodes.Status409Conflict,
        ArgumentException => StatusCodes.Status400BadRequest,
        UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
        KeyNotFoundException => StatusCodes.Status404NotFound,
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
