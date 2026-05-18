using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using HabitApi.Controllers;
using HabitApi.Models.DTO;
using HabitApi.Services.Interfaces;
using Microsoft.AspNetCore.Http;

namespace HabitApi.Tests.Controllers;

/// <summary>
/// Модульные тесты для <see cref="ProfileController"/>.
/// Проверяет получение и обновление профиля текущего пользователя.
/// </summary>
public class ProfileControllerTests
{
    /// <summary>
    /// Создаёт контроллер с замоканным <see cref="IProfileService"/> и заданным пользователем.
    /// </summary>
    private static ProfileController CreateController(Mock<IProfileService> mockService, Guid userId)
    {
        var controller = new ProfileController(mockService.Object);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString())
                }))
            }
        };
        return controller;
    }

    /// <summary>
    /// Проверяет, что при существующем профиле возвращается 200 OK с данными профиля.
    /// </summary>
    [Fact]
    public async Task GetProfile_ReturnsOkWithProfile()
    {
        var userId = Guid.NewGuid();
        var profile = new UserProfileDto
        {
            Email = "test@mail.com",
            Name = "Test User",
            City = "Moscow"
        };

        var mockService = new Mock<IProfileService>();
        mockService.Setup(s => s.GetProfileAsync(userId, It.IsAny<CancellationToken>()))
                   .ReturnsAsync(profile);

        var controller = CreateController(mockService, userId);
        var result = await controller.GetProfile(CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var actual = Assert.IsType<UserProfileDto>(okResult.Value);
        Assert.Equal(profile.Email, actual.Email);
        Assert.Equal(profile.Name, actual.Name);
        Assert.Equal(profile.City, actual.City);
    }

    /// <summary>
    /// Проверяет, что при отсутствии профиля возвращается 404 NotFound.
    /// </summary>
    [Fact]
    public async Task GetProfile_NotFound_ReturnsNotFound()
    {
        var userId = Guid.NewGuid();

        var mockService = new Mock<IProfileService>();
        mockService.Setup(s => s.GetProfileAsync(userId, It.IsAny<CancellationToken>()))
                   .ReturnsAsync((UserProfileDto?)null);

        var controller = CreateController(mockService, userId);
        var result = await controller.GetProfile(CancellationToken.None);

        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    /// <summary>
    /// Проверяет успешное обновление профиля – возвращается 200 OK с обновлёнными данными.
    /// </summary>
    [Fact]
    public async Task UpdateProfile_ValidRequest_ReturnsOk()
    {
        var userId = Guid.NewGuid();
        var request = new UpdateUserProfileDto { Name = "New Name", City = "SPb" };
        var updatedProfile = new UserProfileDto
        {
            Email = "test@mail.com",
            Name = "New Name",
            City = "SPb"
        };

        var mockService = new Mock<IProfileService>();
        mockService.Setup(s => s.UpdateProfileAsync(userId, request, It.IsAny<CancellationToken>()))
                   .ReturnsAsync(updatedProfile);

        var controller = CreateController(mockService, userId);
        var result = await controller.UpdateProfile(request, CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var actual = Assert.IsType<UserProfileDto>(okResult.Value);
        Assert.Equal(updatedProfile.Name, actual.Name);
        Assert.Equal(updatedProfile.City, actual.City);
    }

    /// <summary>
    /// Проверяет, что при обновлении несуществующего профиля возвращается 404 NotFound.
    /// </summary>
    [Fact]
    public async Task UpdateProfile_NotFound_ReturnsNotFound()
    {
        var userId = Guid.NewGuid();
        var request = new UpdateUserProfileDto { City = "Unknown" };

        var mockService = new Mock<IProfileService>();
        mockService.Setup(s => s.UpdateProfileAsync(userId, request, It.IsAny<CancellationToken>()))
                   .ReturnsAsync((UserProfileDto?)null);

        var controller = CreateController(mockService, userId);
        var result = await controller.UpdateProfile(request, CancellationToken.None);

        Assert.IsType<NotFoundObjectResult>(result.Result);
    }
}
