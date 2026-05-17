using Xunit;
using Moq;
using Microsoft.AspNetCore.Identity;
using HabitApi.Models.Domain;
using HabitApi.Models.DTO;
using HabitApi.Services;
using HabitApi.Services.Interfaces;
using HabitApi.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HabitApi.Tests.Services;

/// <summary>
/// Модульные тесты для <see cref="AuthService"/>.
/// Проверяет регистрацию, подтверждение email и вход пользователя.
/// </summary>
public class AuthServiceTests
{
    private const string ValidJwtSecret = "very_long_secret_key_1234567890_1234567890_1234567890";

    // REGISTER
    [Fact]
    public async Task RegisterAsync_ValidRequest_ReturnsRegistrationResponse()
    {
        var request = new RegisterRequestDto { Email = "new@test.com", Password = "Strong1!", City = "Moscow" };

        var userManager = MockUserManager<ApplicationUser>();
        userManager.Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>(), request.Password))
            .Callback<ApplicationUser, string>((u, _) => u.Id = Guid.NewGuid())
            .ReturnsAsync(IdentityResult.Success);
        userManager.Setup(um => um.GenerateEmailConfirmationTokenAsync(It.IsAny<ApplicationUser>()))
                   .ReturnsAsync("test-token");

        var signInManager = MockSignInManager<ApplicationUser>(userManager);
        var emailService = new Mock<IEmailService>();
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "AppBaseUrl", "http://localhost" },
                { "Jwt:Secret", ValidJwtSecret }
            }).Build();
        var logger = new Mock<ILogger<AuthService>>().Object;

        var service = new AuthService(userManager.Object, signInManager.Object, emailService.Object, config, logger);

        var result = await service.RegisterAsync(request, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(request.Email, result.Email);
        Assert.NotEqual(Guid.Empty, result.UserId);
        Assert.Contains("check your email", result.Message);
    }

    [Fact]
    public async Task RegisterAsync_DuplicateEmail_ThrowsConflictException()
    {
        var request = new RegisterRequestDto { Email = "existing@test.com", Password = "Strong1!", City = "Moscow" };

        var userManager = MockUserManager<ApplicationUser>();
        userManager.Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>(), request.Password))
                   .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Duplicate email" }));

        var signInManager = MockSignInManager<ApplicationUser>(userManager);
        var emailService = new Mock<IEmailService>();
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "Jwt:Secret", ValidJwtSecret }
            }).Build();
        var logger = new Mock<ILogger<AuthService>>().Object;

        var service = new AuthService(userManager.Object, signInManager.Object, emailService.Object, config, logger);

        await Assert.ThrowsAsync<ConflictException>(
            () => service.RegisterAsync(request, CancellationToken.None));
    }

    [Fact]
    public async Task RegisterAsync_TooLongEmail_ThrowsArgumentException()
    {
        var longEmail = new string('a', 257) + "@test.com";
        var request = new RegisterRequestDto { Email = longEmail, Password = "Strong1!", City = "Moscow" };

        var userManager = MockUserManager<ApplicationUser>();
        var signInManager = MockSignInManager<ApplicationUser>(userManager);
        var emailService = new Mock<IEmailService>();
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "Jwt:Secret", ValidJwtSecret }
            }).Build();
        var logger = new Mock<ILogger<AuthService>>().Object;

        var service = new AuthService(userManager.Object, signInManager.Object, emailService.Object, config, logger);

        await Assert.ThrowsAsync<ArgumentException>(
            () => service.RegisterAsync(request, CancellationToken.None));
    }

    // LOGIN
    [Fact]
    public async Task LoginAsync_ValidCredentials_ReturnsAuthResponse()
    {
        var request = new LoginRequestDto { Email = "test@test.com", Password = "correct" };
        var user = new ApplicationUser { Id = Guid.NewGuid(), Email = request.Email, EmailConfirmed = true };

        var userManager = MockUserManager<ApplicationUser>();
        userManager.Setup(um => um.FindByEmailAsync(request.Email)).ReturnsAsync(user);
        userManager.Setup(um => um.IsEmailConfirmedAsync(user)).ReturnsAsync(true);

        var signInManager = MockSignInManager<ApplicationUser>(userManager);
        signInManager.Setup(sm => sm.CheckPasswordSignInAsync(user, request.Password, false))
                     .ReturnsAsync(SignInResult.Success);

        var emailService = new Mock<IEmailService>();
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "Jwt:Secret", ValidJwtSecret }
            }).Build();
        var logger = new Mock<ILogger<AuthService>>().Object;

        var service = new AuthService(userManager.Object, signInManager.Object, emailService.Object, config, logger);

        var result = await service.LoginAsync(request, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(user.Id, result.UserId);
        Assert.NotNull(result.AccessToken);
    }

    [Fact]
    public async Task LoginAsync_InvalidPassword_ReturnsNull()
    {
        var request = new LoginRequestDto { Email = "test@test.com", Password = "wrong" };
        var user = new ApplicationUser { Id = Guid.NewGuid(), Email = request.Email, EmailConfirmed = true };

        var userManager = MockUserManager<ApplicationUser>();
        userManager.Setup(um => um.FindByEmailAsync(request.Email)).ReturnsAsync(user);
        userManager.Setup(um => um.IsEmailConfirmedAsync(user)).ReturnsAsync(true);

        var signInManager = MockSignInManager<ApplicationUser>(userManager);
        signInManager.Setup(sm => sm.CheckPasswordSignInAsync(user, request.Password, false))
                     .ReturnsAsync(SignInResult.Failed);

        var emailService = new Mock<IEmailService>();
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "Jwt:Secret", ValidJwtSecret }
            }).Build();
        var logger = new Mock<ILogger<AuthService>>().Object;

        var service = new AuthService(userManager.Object, signInManager.Object, emailService.Object, config, logger);

        var result = await service.LoginAsync(request, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task LoginAsync_EmailNotConfirmed_ThrowsUnauthorizedAccessException()
    {
        var request = new LoginRequestDto { Email = "test@test.com", Password = "correct" };
        var user = new ApplicationUser { Id = Guid.NewGuid(), Email = request.Email, EmailConfirmed = false };

        var userManager = MockUserManager<ApplicationUser>();
        userManager.Setup(um => um.FindByEmailAsync(request.Email)).ReturnsAsync(user);
        userManager.Setup(um => um.IsEmailConfirmedAsync(user)).ReturnsAsync(false);

        var signInManager = MockSignInManager<ApplicationUser>(userManager);
        var emailService = new Mock<IEmailService>();
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "Jwt:Secret", ValidJwtSecret }
            }).Build();
        var logger = new Mock<ILogger<AuthService>>().Object;

        var service = new AuthService(userManager.Object, signInManager.Object, emailService.Object, config, logger);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => service.LoginAsync(request, CancellationToken.None));
    }

    // CONFIRM EMAIL
    [Fact]
    public async Task ConfirmEmailAsync_ValidToken_ReturnsUser()
    {
        var userId = Guid.NewGuid();
        var token = "valid-token";
        var user = new ApplicationUser { Id = userId, Email = "test@test.com" };

        var userManager = MockUserManager<ApplicationUser>();
        userManager.Setup(um => um.FindByIdAsync(userId.ToString())).ReturnsAsync(user);
        userManager.Setup(um => um.ConfirmEmailAsync(user, token)).ReturnsAsync(IdentityResult.Success);

        var signInManager = MockSignInManager<ApplicationUser>(userManager);
        var emailService = new Mock<IEmailService>();
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "Jwt:Secret", ValidJwtSecret }
            }).Build();
        var logger = new Mock<ILogger<AuthService>>().Object;

        var service = new AuthService(userManager.Object, signInManager.Object, emailService.Object, config, logger);

        var result = await service.ConfirmEmailAsync(userId, token);

        Assert.NotNull(result);
        Assert.Equal(userId, result.Id);
    }

    [Fact]
    public async Task ConfirmEmailAsync_InvalidToken_ReturnsNull()
    {
        var userId = Guid.NewGuid();
        var token = "invalid-token";
        var user = new ApplicationUser { Id = userId, Email = "test@test.com" };

        var userManager = MockUserManager<ApplicationUser>();
        userManager.Setup(um => um.FindByIdAsync(userId.ToString())).ReturnsAsync(user);
        userManager.Setup(um => um.ConfirmEmailAsync(user, token))
                   .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Invalid token" }));

        var signInManager = MockSignInManager<ApplicationUser>(userManager);
        var emailService = new Mock<IEmailService>();
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "Jwt:Secret", ValidJwtSecret }
            }).Build();
        var logger = new Mock<ILogger<AuthService>>().Object;

        var service = new AuthService(userManager.Object, signInManager.Object, emailService.Object, config, logger);

        var result = await service.ConfirmEmailAsync(userId, token);

        Assert.Null(result);
    }

    // HELPERS
    private static Mock<UserManager<TUser>> MockUserManager<TUser>() where TUser : class
    {
        var store = new Mock<IUserStore<TUser>>();
        return new Mock<UserManager<TUser>>(
            store.Object, null!, null!, null!, null!, null!, null!, null!, null!);
    }

    private static Mock<SignInManager<TUser>> MockSignInManager<TUser>(Mock<UserManager<TUser>> userManager) where TUser : class
    {
        return new Mock<SignInManager<TUser>>(
            userManager.Object,
            Mock.Of<IHttpContextAccessor>(),
            Mock.Of<IUserClaimsPrincipalFactory<TUser>>(),
            null!, null!, null!, null!);
    }
}
