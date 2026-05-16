using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using HabitApi.Controllers;
using HabitApi.Models.DTO;
using HabitApi.Services.Interfaces;

namespace HabitApi.Tests.Controllers;

public class AuthControllerTests
{
    [Fact]
    public async Task Login_ValidCredentials_ReturnsOkWithToken()
    {
        var request = new LoginRequestDto
        {
            Email = "test@example.com",
            Password = "Strong1!"
        };

        var token = "jwt_token";
        var mockAuth = new Mock<IAuthService>();
            
        mockAuth
            .Setup(s => s.LoginAsync(It.IsAny<LoginRequestDto>(), It.IsAny<CancellationToken>()));

        var controller = new AuthController(mockAuth.Object);

        var result = await controller.Login(request, CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(ok.Value);
    }

    [Fact]
    public async Task Login_InvalidCredentials_ReturnsUnauthorized()
    {
        var request = new LoginRequestDto
        {
            Email = "wrong@mail.com",
            Password = "wrong"
        };

        var mockAuth = new Mock<IAuthService>();

        mockAuth
            .Setup(s => s.LoginAsync(request, It.IsAny<CancellationToken>()));

        var controller = new AuthController(mockAuth.Object);

        var result = await controller.Login(request, CancellationToken.None);

        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Fact]
    public async Task Register_ValidRequest_ReturnsCreated()
    {
        var request = new RegisterRequestDto
        {
            Email = "new@mail.com",
            Password = "Strong1!",
            City = "Moscow"
        };

        var mockAuth = new Mock<IAuthService>();

        mockAuth
            .Setup(s => s.RegisterAsync(request, It.IsAny<CancellationToken>()));

        var controller = new AuthController(mockAuth.Object);

        var result = await controller.Register(request, CancellationToken.None);

        Assert.IsType<CreatedResult>(result);
    }

    [Fact]
    public async Task Register_ServiceReturnsFalse_ReturnsBadRequest()
    {
        var request = new RegisterRequestDto
        {
            Email = "",
            Password = "",
            City = ""
        };

        var mockAuth = new Mock<IAuthService>();

        mockAuth
            .Setup(s => s.RegisterAsync(request, It.IsAny<CancellationToken>()));

        var controller = new AuthController(mockAuth.Object);

        var result = await controller.Register(request, CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result);
    }
}