using HabitApi.Data;
using HabitApi.Services;
using HabitApi.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using DotNetEnv;
using Microsoft.Extensions.DependencyInjection;

// Загружаем .env файл
Env.Load();

var builder = WebApplication.CreateBuilder(args);

// --- Настройка строки подключения к БД из переменных окружения ---
var host = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";
var port = Environment.GetEnvironmentVariable("DB_PORT") ?? "5432";
var database = Environment.GetEnvironmentVariable("DB_NAME") ?? "habit_tracker";
var username = Environment.GetEnvironmentVariable("DB_USER") ?? "habit_user";
var password = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "default_password";
var connectionString = $"Host={host};Port={port};Database={database};Username={username};Password={password}";

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// --- JWT аутентификация ---
var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET")
                ?? builder.Configuration["Jwt:Secret"]
                ?? throw new InvalidOperationException("JWT_SECRET не задан в .env или конфигурации");
var key = Encoding.UTF8.GetBytes(jwtSecret);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "HabitApi",
            ValidAudience = builder.Configuration["Jwt:Audience"] ?? "HabitApiClient",
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

builder.Services.AddAuthorization();

// --- Добавляем HttpClient для вызовов внешних API ---
builder.Services.AddHttpClient();

// --- Регистрируем сервисы ---
builder.Services.AddScoped<IWeatherService, WeatherService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IHabitService, HabitService>();
builder.Services.AddScoped<IHabitEntryService, HabitEntryService>();
builder.Services.AddScoped<IStatsService, StatsService>();
builder.Services.AddScoped<IAiInsightsService, AiInsightsService>();

builder.Services.AddControllers();

var app = builder.Build();

// --- Middleware ---
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Запуск
app.Run();
