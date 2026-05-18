using HabitApi.Controllers;
using HabitApi.Exceptions;
using HabitApi.Models.Domain;
using HabitApi.Models.DTO;
using HabitApi.Services.Interfaces;
using HabitApi.Tests.Integration.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace HabitApi.Tests.Integration.Backend;

[Collection(BackendIntegrationCollection.Name)]
[Trait("Category", "Integration")]
[Trait("Dependency", "Postgres")]
public sealed class BackendWorkflowIntegrationTests
{
    private readonly PostgresContainerFixture _postgres;

    public BackendWorkflowIntegrationTests(PostgresContainerFixture postgres)
    {
        _postgres = postgres;
    }

    [BackendIntegrationFact]
    public async Task AuthAndProfileWorkflow_UsesRealIdentityTablesInPostgres()
    {
        await _postgres.ResetDatabaseAsync();
        await using var provider = BackendTestServiceFactory.Create(_postgres.ConnectionString);
        using var scope = provider.CreateScope();

        var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();
        var profileService = scope.ServiceProvider.GetRequiredService<IProfileService>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var emailService = (TestEmailService)scope.ServiceProvider.GetRequiredService<IEmailService>();

        var email = UniqueEmail();
        var registration = await authService.RegisterAsync(
            new RegisterRequestDto { Email = email, Password = "password1", City = "Samara" },
            CancellationToken.None);

        Assert.Equal(email, registration.Email);
        Assert.Single(emailService.SentMessages);

        var user = await userManager.FindByEmailAsync(email);
        Assert.NotNull(user);

        var loginBeforeConfirmation = await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            authService.LoginAsync(new LoginRequestDto { Email = email, Password = "password1" }, CancellationToken.None));
        Assert.Contains("Email not confirmed", loginBeforeConfirmation.Message);

        var token = await userManager.GenerateEmailConfirmationTokenAsync(user!);
        var confirmedUser = await authService.ConfirmEmailAsync(user!.Id, token);
        Assert.NotNull(confirmedUser);

        var login = await authService.LoginAsync(
            new LoginRequestDto { Email = email, Password = "password1" },
            CancellationToken.None);

        Assert.NotNull(login);
        Assert.False(string.IsNullOrWhiteSpace(login!.AccessToken));

        var profile = await profileService.UpdateProfileAsync(
            user.Id,
            new UpdateUserProfileDto
            {
                Name = "Integration User",
                City = "Kazan",
                HabitReminderEnabled = true,
                HabitReminderTime = "08:30",
                ThemePreference = "dark"
            },
            CancellationToken.None);

        Assert.NotNull(profile);
        Assert.Equal("Integration User", profile!.Name);
        Assert.Equal("Kazan", profile.City);
        Assert.True(profile.HabitReminderEnabled);
        Assert.Equal("08:30", profile.HabitReminderTime);
        Assert.Equal("dark", profile.ThemePreference);
    }

    [BackendIntegrationFact]
    public async Task HabitEntryAndDailyStatsWorkflow_PersistsAndAggregatesWithPostgres()
    {
        await _postgres.ResetDatabaseAsync();
        await using var provider = BackendTestServiceFactory.Create(_postgres.ConnectionString);
        using var scope = provider.CreateScope();

        var user = await CreateConfirmedUserAsync(scope.ServiceProvider, UniqueEmail(), "Samara");
        var habits = scope.ServiceProvider.GetRequiredService<IHabitService>();
        var entries = scope.ServiceProvider.GetRequiredService<IHabitEntryService>();
        var stats = scope.ServiceProvider.GetRequiredService<IStatsService>();
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var habit = await habits.CreateHabitAsync(
            user.Id,
            new CreateHabitDto
            {
                Name = "Drink water",
                IsPositive = true,
                TriggerType = TriggerType.CountPerDay,
                TriggerValue = "8",
                TargetDays = 30,
                Reminders = new List<string> { "09:00" }
            },
            CancellationToken.None);

        var entry = await entries.AddHabitEntryAsync(
            user.Id,
            habit.Id,
            new CreateHabitEntryDto
            {
                Date = today,
                Status = HabitEntryStatus.Completed,
                Note = "Done"
            },
            CancellationToken.None);

        Assert.Equal(HabitEntryStatus.Completed, entry.Status);

        await Assert.ThrowsAsync<ConflictException>(() =>
            entries.AddHabitEntryAsync(
                user.Id,
                habit.Id,
                new CreateHabitEntryDto { Date = today, Status = HabitEntryStatus.Skipped },
                CancellationToken.None));

        var summary = await stats.GetDailySummaryAsync(user.Id, today, CancellationToken.None);
        Assert.Equal(1, summary.HabitsCompleted);
        Assert.Equal(0, summary.HabitsSkipped);
        Assert.Equal("Clouds", summary.Weather?.Condition);
        Assert.Contains("Completed: 1", summary.AiInsight);

        var deleted = await habits.DeleteHabitAsync(user.Id, habit.Id, CancellationToken.None);
        Assert.True(deleted);

        var activeHabits = await habits.GetUserHabitsAsync(user.Id, CancellationToken.None);
        Assert.Empty(activeHabits);
    }

    [BackendIntegrationFact]
    public async Task HabitData_IsIsolatedBetweenUsers()
    {
        await _postgres.ResetDatabaseAsync();
        await using var provider = BackendTestServiceFactory.Create(_postgres.ConnectionString);
        using var scope = provider.CreateScope();

        var owner = await CreateConfirmedUserAsync(scope.ServiceProvider, UniqueEmail(), "Samara");
        var other = await CreateConfirmedUserAsync(scope.ServiceProvider, UniqueEmail(), "Samara");
        var habits = scope.ServiceProvider.GetRequiredService<IHabitService>();
        var entries = scope.ServiceProvider.GetRequiredService<IHabitEntryService>();

        var ownerHabit = await habits.CreateHabitAsync(
            owner.Id,
            new CreateHabitDto
            {
                Name = "Private habit",
                TriggerType = TriggerType.TimeOfDay,
                TriggerValue = "07:30"
            },
            CancellationToken.None);

        var visibleToOwner = await habits.GetHabitByIdAsync(owner.Id, ownerHabit.Id, CancellationToken.None);
        var visibleToOther = await habits.GetHabitByIdAsync(other.Id, ownerHabit.Id, CancellationToken.None);
        var otherEntries = await entries.GetHabitEntriesAsync(other.Id, ownerHabit.Id, null, null, CancellationToken.None);

        Assert.NotNull(visibleToOwner);
        Assert.Null(visibleToOther);
        Assert.Empty(otherEntries);
        Assert.False(await habits.DeleteHabitAsync(other.Id, ownerHabit.Id, CancellationToken.None));
    }

    [BackendIntegrationFact]
    public async Task CitySummary_AggregatesDistinctUsersByHabitName()
    {
        await _postgres.ResetDatabaseAsync();
        await using var provider = BackendTestServiceFactory.Create(_postgres.ConnectionString);
        using var scope = provider.CreateScope();

        var firstUser = await CreateConfirmedUserAsync(scope.ServiceProvider, UniqueEmail(), "Samara");
        var secondUser = await CreateConfirmedUserAsync(scope.ServiceProvider, UniqueEmail(), "Samara");
        var habits = scope.ServiceProvider.GetRequiredService<IHabitService>();
        var entries = scope.ServiceProvider.GetRequiredService<IHabitEntryService>();
        var stats = scope.ServiceProvider.GetRequiredService<IStatsService>();
        var entryDate = PreviousWeekStart();

        var firstHabit = await habits.CreateHabitAsync(
            firstUser.Id,
            new CreateHabitDto { Name = "Reading", TriggerType = TriggerType.CountPerDay, TriggerValue = "1" },
            CancellationToken.None);
        var secondHabit = await habits.CreateHabitAsync(
            secondUser.Id,
            new CreateHabitDto { Name = "Reading", TriggerType = TriggerType.CountPerDay, TriggerValue = "1" },
            CancellationToken.None);

        await entries.AddHabitEntryAsync(
            firstUser.Id,
            firstHabit.Id,
            new CreateHabitEntryDto { Date = entryDate, Status = HabitEntryStatus.Completed },
            CancellationToken.None);
        await entries.AddHabitEntryAsync(
            secondUser.Id,
            secondHabit.Id,
            new CreateHabitEntryDto { Date = entryDate, Status = HabitEntryStatus.Completed },
            CancellationToken.None);

        var citySummary = await stats.GetWeeklyCitySummaryAsync("Samara", CancellationToken.None);
        var reading = Assert.Single(citySummary.PopularHabits, habit => habit.HabitName == "Reading");

        Assert.Equal(2, reading.UserCount);
        Assert.Equal(2, reading.TotalUsers);
        Assert.Equal(100, reading.Percentage);
    }

    [BackendIntegrationFact]
    public async Task WeatherController_MapsProviderNotFoundAndRateLimitToHttpStatuses()
    {
        await using var provider = BackendTestServiceFactory.Create(_postgres.ConnectionString);
        using var scope = provider.CreateScope();

        var weatherService = scope.ServiceProvider.GetRequiredService<IWeatherService>();
        var controller = new WeatherController(weatherService);
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var missingCityResult = await controller.GetWeather("InvalidCity", today, CancellationToken.None);
        Assert.IsType<NotFoundObjectResult>(missingCityResult.Result);

        var rateLimitResult = await controller.GetWeather("ratelimit", today, CancellationToken.None);
        var statusResult = Assert.IsType<ObjectResult>(rateLimitResult.Result);
        Assert.Equal(StatusCodes.Status429TooManyRequests, statusResult.StatusCode);
    }

    private static async Task<ApplicationUser> CreateConfirmedUserAsync(
        IServiceProvider serviceProvider,
        string email,
        string city)
    {
        var authService = serviceProvider.GetRequiredService<IAuthService>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        await authService.RegisterAsync(
            new RegisterRequestDto { Email = email, Password = "password1", City = city },
            CancellationToken.None);

        var user = await userManager.FindByEmailAsync(email);
        Assert.NotNull(user);

        var token = await userManager.GenerateEmailConfirmationTokenAsync(user!);
        var confirmed = await authService.ConfirmEmailAsync(user!.Id, token);
        Assert.NotNull(confirmed);

        return user;
    }

    private static DateOnly PreviousWeekStart()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        return today.AddDays(-(int)today.DayOfWeek + 1 - 7);
    }

    private static string UniqueEmail() =>
        $"integration-{Guid.NewGuid():N}@example.com";
}
