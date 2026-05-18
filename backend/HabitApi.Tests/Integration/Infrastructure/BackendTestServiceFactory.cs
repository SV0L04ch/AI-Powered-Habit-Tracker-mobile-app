using HabitApi.Data;
using HabitApi.Models.Domain;
using HabitApi.Services;
using HabitApi.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HabitApi.Tests.Integration.Infrastructure;

internal static class BackendTestServiceFactory
{
    private const string JwtSecret = "integration-test-secret-value-32-chars-minimum";

    public static ServiceProvider Create(string connectionString)
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Secret"] = JwtSecret,
                ["Jwt:Issuer"] = "HabitApi",
                ["Jwt:Audience"] = "HabitApiClient",
                ["AppBaseUrl"] = "http://localhost"
            })
            .Build();

        services.AddSingleton<IConfiguration>(configuration);
        services.AddLogging(builder => builder.AddConsole());
        services.AddHttpContextAccessor();
        services.AddDataProtection();
        services.AddOptions();
        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie();

        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

        services.AddIdentityCore<ApplicationUser>(options =>
            {
                options.SignIn.RequireConfirmedEmail = true;
                options.User.RequireUniqueEmail = true;
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders()
            .AddSignInManager();

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IHabitService, HabitService>();
        services.AddScoped<IHabitEntryService, HabitEntryService>();
        services.AddScoped<IProfileService, ProfileService>();
        services.AddScoped<IStatsService, StatsService>();
        services.AddSingleton<IEmailService, TestEmailService>();
        services.AddSingleton<IWeatherService, TestWeatherService>();
        services.AddSingleton<IAiInsightsService, TestAiInsightsService>();

        return services.BuildServiceProvider(validateScopes: true);
    }
}
