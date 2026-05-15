using Xunit;
using Moq;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using HabitApi.Data;
using HabitApi.Models.Domain;
using HabitApi.Models.DTO;
using HabitApi.Services;

namespace HabitApi.Tests.Services;

public class AuthServiceTests
{
    [Fact]
    public async Task LoginAsync_InvalidPassword_ReturnsNull()
    {
        var email = "test@test.com";
        var password = "wrong";
        var user = new User { Email = email };

        var userStore = new Mock<IUserStore<User>>();
        var userManagerMock = new Mock<UserManager<User>>(userStore.Object, null!, null!, null!, null!, null!, null!, null!, null!);
        userManagerMock.Setup(um => um.FindByEmailAsync(email)).ReturnsAsync(user);

        var signInManagerMock = new Mock<SignInManager<User>>(
            userManagerMock.Object,
            Mock.Of<IHttpContextAccessor>(),
            Mock.Of<IUserClaimsPrincipalFactory<User>>(),
            null!, null!, null!, null!);
        signInManagerMock.Setup(sm => sm.CheckPasswordSignInAsync(user, password, false))
                         .ReturnsAsync(SignInResult.Failed);

        var mockContext = new Mock<AppDbContext>();
        var mockConfig = new Mock<IConfiguration>();
        mockConfig.Setup(c => c["Jwt:Secret"]).Returns("secret1234567890");

        var service = new AuthService(userManagerMock.Object, signInManagerMock.Object, mockContext.Object, mockConfig.Object);
        var result = await service.LoginAsync(new LoginRequestDto { Email = email, Password = password });

        Assert.Null(result);
    }

    [Fact]
    public async Task RegisterAsync_ValidRequest_ReturnsTrue()
    {
        var request = new RegisterRequestDto { Email = "new@test.com", Password = "Strong1!", UserName = "user" };
        var userStore = new Mock<IUserStore<User>>();
        var userManagerMock = new Mock<UserManager<User>>(userStore.Object, null!, null!, null!, null!, null!, null!, null!, null!);
        userManagerMock.Setup(um => um.CreateAsync(It.IsAny<User>(), request.Password))
                       .ReturnsAsync(IdentityResult.Success);

        var signInManagerMock = new Mock<SignInManager<User>>(
            userManagerMock.Object,
            Mock.Of<IHttpContextAccessor>(),
            Mock.Of<IUserClaimsPrincipalFactory<User>>(),
            null!, null!, null!, null!);

        var mockContext = new Mock<AppDbContext>();
        var mockConfig = new Mock<IConfiguration>();

        var service = new AuthService(userManagerMock.Object, signInManagerMock.Object, mockContext.Object, mockConfig.Object);
        var result = await service.RegisterAsync(request);

        Assert.True(result);
    }
}