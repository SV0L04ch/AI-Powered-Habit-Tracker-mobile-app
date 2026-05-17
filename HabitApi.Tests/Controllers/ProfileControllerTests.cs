using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using HabitApi.Controllers;
using HabitApi.Models.DTO;
using HabitApi.Services.Interfaces;

namespace HabitApi.Tests.Controllers;

public class ProfileControllerTests
{
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

    [Fact]
    public async Task GetProfile_ReturnsOkWithProfile()
    {
        var userId = Guid.NewGuid();
        var profile = new UserProfileDto { UserName = "test", Email = "test@mail.com", City = "Moscow" };

        var mockService = new Mock<IProfileService>();
        mockService.Setup(s => s.GetProfileAsync(userId, It.IsAny<CancellationToken>()))
                   .ReturnsAsync(profile);

        var controller = CreateController(mockService, userId);
        var result = await controller.GetProfile(CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(profile, ok.Value);
    }

    [Fact]
    public async Task UpdateProfile_ValidRequest_ReturnsOk()
    {
        var userId = Guid.NewGuid();
        var request = new UpdateUserProfileDto { City = "SPb" };
        var updatedProfile = new UserProfileDto { City = "SPb" };

        var mockService = new Mock<IProfileService>();
        mockService.Setup(s => s.UpdateProfileAsync(userId, request, It.IsAny<CancellationToken>()))
                   .ReturnsAsync(updatedProfile);

        var controller = CreateController(mockService, userId);
        var result = await controller.UpdateProfile(request, CancellationToken.None);

        Assert.IsType<OkObjectResult>(result.Result);
    }
}