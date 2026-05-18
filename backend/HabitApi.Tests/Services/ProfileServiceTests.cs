using Xunit;
using Moq;
using Microsoft.AspNetCore.Identity;
using HabitApi.Models.Domain;
using HabitApi.Models.DTO;
using HabitApi.Services;

namespace HabitApi.Tests.Services;

/// <summary>
/// Модульные тесты для <see cref="ProfileService"/>.
/// Проверяет чтение и обновление профиля текущего пользователя.
/// </summary>
public class ProfileServiceTests
{
    /// <summary>
    /// Создаёт замоканный <see cref="UserManager{TUser}"/> для тестов.
    /// </summary>
    private static Mock<UserManager<ApplicationUser>> MockUserManager()
    {
        var store = new Mock<IUserStore<ApplicationUser>>();
        return new Mock<UserManager<ApplicationUser>>(
            store.Object, null!, null!, null!, null!, null!, null!, null!, null!);
    }

    /// <summary>
    /// Проверяет успешное обновление города пользователя и возврат обновлённого DTO.
    /// </summary>
    [Fact]
    public async Task UpdateProfileAsync_UpdatesCityAndReturnsDto()
    {
        var userId = Guid.NewGuid();
        var request = new UpdateUserProfileDto { City = "Казань" };
        var user = new ApplicationUser
        {
            Id = userId,
            Email = "test@test.com",
            City = "Москва",
            UserName = "test@test.com"
        };

        var userManager = MockUserManager();
        userManager.Setup(um => um.FindByIdAsync(userId.ToString()))
                   .ReturnsAsync(user);
        userManager.Setup(um => um.UpdateAsync(It.IsAny<ApplicationUser>()))
                   .ReturnsAsync(IdentityResult.Success);

        var service = new ProfileService(userManager.Object);

        var result = await service.UpdateProfileAsync(userId, request, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("Казань", result.City);
        userManager.Verify(um => um.UpdateAsync(It.IsAny<ApplicationUser>()), Times.Once);
    }

    /// <summary>
    /// Проверяет, что при отсутствии пользователя возвращается null.
    /// </summary>
    [Fact]
    public async Task GetProfileAsync_NotFound_ReturnsNull()
    {
        var userId = Guid.NewGuid();
        var userManager = MockUserManager();
        userManager.Setup(um => um.FindByIdAsync(userId.ToString()))
                   .ReturnsAsync((ApplicationUser?)null);

        var service = new ProfileService(userManager.Object);

        var result = await service.GetProfileAsync(userId, CancellationToken.None);

        Assert.Null(result);
    }

    /// <summary>
    /// Проверяет успешное получение профиля существующего пользователя.
    /// </summary>
    [Fact]
    public async Task GetProfileAsync_ExistingUser_ReturnsDto()
    {
        var userId = Guid.NewGuid();
        var user = new ApplicationUser
        {
            Id = userId,
            Email = "test@test.com",
            City = "Москва",
            UserName = "test_user",
            Name = "Test User",
            ThemePreference = "dark"
        };

        var userManager = MockUserManager();
        userManager.Setup(um => um.FindByIdAsync(userId.ToString()))
                   .ReturnsAsync(user);

        var service = new ProfileService(userManager.Object);

        var result = await service.GetProfileAsync(userId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("test@test.com", result.Email);
        Assert.Equal("Test User", result.Name);
        Assert.Equal("dark", result.ThemePreference);
    }
}
