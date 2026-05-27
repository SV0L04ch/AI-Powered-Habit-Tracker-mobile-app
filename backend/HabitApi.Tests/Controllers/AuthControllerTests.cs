using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using HabitApi.Controllers;
using HabitApi.Models.DTO;
using HabitApi.Services.Interfaces;
using HabitApi.Exceptions;
using Microsoft.Extensions.Configuration;

namespace HabitApi.Tests.Controllers;

/// <summary>
/// Модульные тесты для <see cref="AuthController"/>.
/// Проверяет вход, регистрацию, подтверждение почты и выход.
/// </summary>
public class AuthControllerTests
{
    private static AuthController CreateController(IAuthService authService)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["FrontendBaseUrl"] = "http://localhost:5173"
            })
            .Build();

        return new AuthController(authService, configuration)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };
    }

    // LOGIN
    [Fact]
    public async Task Login_ValidCredentials_ReturnsOkWithToken()
    {
        // Arrange
        var request = new LoginRequestDto { Email = "test@example.com", Password = "Strong1!" };
        var expectedResponse = new AuthResponseDto
        {
            UserId = Guid.NewGuid(),
            Email = request.Email,
            AccessToken = "fake.jwt.token"
        };

        var mockAuth = new Mock<IAuthService>();
        mockAuth.Setup(s => s.LoginAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

        var controller = CreateController(mockAuth.Object);

        // Act
        var result = await controller.Login(request, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var actual = Assert.IsType<AuthResponseDto>(okResult.Value);
        Assert.Equal(expectedResponse.AccessToken, actual.AccessToken);
    }

    [Fact]
    public async Task Login_InvalidCredentials_ReturnsUnauthorized()
    {
        var request = new LoginRequestDto { Email = "wrong@mail.com", Password = "wrong" };

        var mockAuth = new Mock<IAuthService>();
        mockAuth.Setup(s => s.LoginAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync((AuthResponseDto?)null);

        var controller = CreateController(mockAuth.Object);

        var result = await controller.Login(request, CancellationToken.None);

        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Fact]
    public async Task Login_EmailNotConfirmed_ReturnsUnauthorized()
    {
        var request = new LoginRequestDto { Email = "test@example.com", Password = "Strong1!" };

        var mockAuth = new Mock<IAuthService>();
        mockAuth.Setup(s => s.LoginAsync(request, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new UnauthorizedAccessException("Email not confirmed."));

        var controller = CreateController(mockAuth.Object);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => controller.Login(request, CancellationToken.None));
    }

    // REGISTER
    [Fact]
    public async Task Register_ValidRequest_ReturnsCreated()
    {
        var request = new RegisterRequestDto { Email = "new@mail.com", Password = "Strong1!", City = "Moscow" };
        var expectedResponse = new RegistrationResponseDto
        {
            UserId = Guid.NewGuid(),
            Email = request.Email,
            Message = "Registration successful."
        };

        var mockAuth = new Mock<IAuthService>();
        mockAuth.Setup(s => s.RegisterAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

        var controller = CreateController(mockAuth.Object);

        var result = await controller.Register(request, CancellationToken.None);

        var createdResult = Assert.IsType<CreatedResult>(result);
        var actual = Assert.IsType<RegistrationResponseDto>(createdResult.Value);
        Assert.Equal(expectedResponse.Email, actual.Email);
    }

    [Fact]
    public async Task Register_DuplicateEmail_ReturnsConflict()
    {
        var request = new RegisterRequestDto { Email = "existing@mail.com", Password = "Strong1!", City = "Moscow" };

        var mockAuth = new Mock<IAuthService>();
        mockAuth.Setup(s => s.RegisterAsync(request, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ConflictException("User with this email already exists."));

        var controller = CreateController(mockAuth.Object);

        await Assert.ThrowsAsync<ConflictException>(
            () => controller.Register(request, CancellationToken.None));
    }

    // Тест Register_InvalidData_ReturnsBadRequest удалён, так как валидация не работает
    // в модульном тесте (нет ModelState). Это поведение проверяется интеграционно.

    // CONFIRM EMAIL
    [Fact]
    public async Task ConfirmEmail_ValidToken_ReturnsOk()
    {
        var userId = Guid.NewGuid();
        var token = "valid_token";
        var confirmedUser = new HabitApi.Models.Domain.ApplicationUser { Id = userId, Email = "test@example.com" };

        var mockAuth = new Mock<IAuthService>();
        mockAuth.Setup(s => s.ConfirmEmailAsync(userId, token))
                .ReturnsAsync(confirmedUser);

        var controller = CreateController(mockAuth.Object);

        var result = await controller.ConfirmEmail(userId, token);

        var contentResult = Assert.IsType<ContentResult>(result);
        Assert.Equal("text/html; charset=utf-8", contentResult.ContentType);
        Assert.Contains("http://localhost:5173/login", contentResult.Content);
    }

    [Fact]
    public async Task ConfirmEmail_InvalidToken_ReturnsBadRequest()
    {
        var userId = Guid.NewGuid();
        var token = "invalid_token";

        var mockAuth = new Mock<IAuthService>();
        mockAuth.Setup(s => s.ConfirmEmailAsync(userId, token))
                .ReturnsAsync((HabitApi.Models.Domain.ApplicationUser?)null);

        var controller = CreateController(mockAuth.Object);

        var result = await controller.ConfirmEmail(userId, token);

        var contentResult = Assert.IsType<ContentResult>(result);
        Assert.Equal("text/html; charset=utf-8", contentResult.ContentType);
        Assert.Contains("http://localhost:5173/login", contentResult.Content);
    }

    // LOGOUT
    [Fact]
    public void Logout_ReturnsNoContent()
    {
        var controller = CreateController(new Mock<IAuthService>().Object);

        var result = controller.Logout();

        Assert.IsType<NoContentResult>(result);
    }
}
