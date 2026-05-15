using Xunit;
using Moq;
using Microsoft.AspNetCore.Identity;
using HabitApi.Data;
using HabitApi.Models.Domain;
using HabitApi.Models.DTO;
using HabitApi.Services;

namespace HabitApi.Tests.Services;

public class ProfileServiceTests
{
    [Fact]
    public async Task UpdateProfileAsync_UpdatesCityAndReturnsDto()
    {
        var userId = Guid.NewGuid();
        var request = new UpdateUserProfileDto { City = "Казань" };
        var user = new User { Id = userId, City = "Москва" };

        var userStore = new Mock<IUserStore<User>>();
        var userManagerMock = new Mock<UserManager<User>>(userStore.Object, null!, null!, null!, null!, null!, null!, null!, null!);
        userManagerMock.Setup(um => um.FindByIdAsync(userId.ToString())).ReturnsAsync(user);
        userManagerMock.Setup(um => um.UpdateAsync(It.IsAny<User>())).ReturnsAsync(IdentityResult.Success);

        var mockContext = new Mock<AppDbContext>();
        var service = new ProfileService(userManagerMock.Object, mockContext.Object);

        var result = await service.UpdateProfileAsync(userId, request);

        Assert.Equal("Казань", result.City);
        userManagerMock.Verify(um => um.UpdateAsync(It.IsAny<User>()), Times.Once);
    }
}