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
/// Модульные тесты для <see cref="HabitsController"/>.
/// Проверяет создание, получение, обновление и удаление привычек.
/// </summary>
public class HabitsControllerTests
{
    /// <summary>
    /// Создаёт экземпляр контроллера с замоканным сервисом и подставленным пользователем.
    /// </summary>
    /// <param name="mockService">Мок сервиса <see cref="IHabitService"/>.</param>
    /// <param name="userId">Идентификатор пользователя (по умолчанию генерируется новый).</param>
    private static HabitsController CreateController(Mock<IHabitService> mockService, Guid? userId = null)
    {
        var controller = new HabitsController(mockService.Object);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, (userId ?? Guid.NewGuid()).ToString())
                }))
            }
        };
        return controller;
    }

    /// <summary>
    /// Проверяет, что при корректных данных контроллер возвращает 201 CreatedAtAction с созданной привычкой.
    /// </summary>
    [Fact]
    public async Task CreateHabit_ValidDto_ReturnsCreatedAtAction()
    {
        var request = new CreateHabitDto { Name = "Тест" };
        var dto = new HabitDto { Id = Guid.NewGuid(), Name = "Тест" };

        var mockService = new Mock<IHabitService>();
        mockService.Setup(s => s.CreateHabitAsync(It.IsAny<Guid>(), request, It.IsAny<CancellationToken>()))
                   .ReturnsAsync(dto);

        var controller = CreateController(mockService);
        var result = await controller.CreateHabit(request, CancellationToken.None);

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(dto, created.Value);
    }

    /// <summary>
    /// Проверяет, что при существующей привычке возвращается 200 OK с объектом привычки.
    /// </summary>
    [Fact]
    public async Task GetHabitById_ExistingHabit_ReturnsOk()
    {
        var habitId = Guid.NewGuid();
        var dto = new HabitDto { Id = habitId, Name = "Тест" };

        var mockService = new Mock<IHabitService>();
        mockService.Setup(s => s.GetHabitByIdAsync(It.IsAny<Guid>(), habitId, It.IsAny<CancellationToken>()))
                   .ReturnsAsync(dto);

        var controller = CreateController(mockService);
        var result = await controller.GetHabitById(habitId, CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(dto, ok.Value);
    }

    /// <summary>
    /// Проверяет, что при отсутствии привычки возвращается 404 NotFound.
    /// </summary>
    [Fact]
    public async Task GetHabitById_NotFound_ReturnsNotFound()
    {
        var habitId = Guid.NewGuid();
        var mockService = new Mock<IHabitService>();
        mockService.Setup(s => s.GetHabitByIdAsync(It.IsAny<Guid>(), habitId, It.IsAny<CancellationToken>()))
                   .ReturnsAsync((HabitDto?)null);

        var controller = CreateController(mockService);
        var result = await controller.GetHabitById(habitId, CancellationToken.None);

        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    /// <summary>
    /// Проверяет успешное обновление привычки – возвращается 200 OK с обновлённым DTO.
    /// </summary>
    [Fact]
    public async Task UpdateHabit_ValidRequest_ReturnsOk()
    {
        var habitId = Guid.NewGuid();
        var updateDto = new UpdateHabitDto { Name = "Обновлённое имя" };
        var expectedDto = new HabitDto { Id = habitId, Name = "Обновлённое имя" };

        var mockService = new Mock<IHabitService>();
        mockService.Setup(s => s.UpdateHabitAsync(It.IsAny<Guid>(), habitId, updateDto, It.IsAny<CancellationToken>()))
                   .ReturnsAsync(expectedDto);

        var controller = CreateController(mockService);
        var result = await controller.UpdateHabit(habitId, updateDto, CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(expectedDto, okResult.Value);
    }

    /// <summary>
    /// Проверяет, что при обновлении несуществующей привычки возвращается 404 NotFound.
    /// </summary>
    [Fact]
    public async Task UpdateHabit_NotFound_ReturnsNotFound()
    {
        var habitId = Guid.NewGuid();
        var updateDto = new UpdateHabitDto { Name = "Неважно" };

        var mockService = new Mock<IHabitService>();
        mockService.Setup(s => s.UpdateHabitAsync(It.IsAny<Guid>(), habitId, updateDto, It.IsAny<CancellationToken>()))
                   .ReturnsAsync((HabitDto?)null);

        var controller = CreateController(mockService);
        var result = await controller.UpdateHabit(habitId, updateDto, CancellationToken.None);

        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    /// <summary>
    /// Проверяет успешное мягкое удаление привычки – возвращается 204 NoContent.
    /// </summary>
    [Fact]
    public async Task DeleteHabit_ValidId_ReturnsNoContent()
    {
        var habitId = Guid.NewGuid();

        var mockService = new Mock<IHabitService>();
        mockService.Setup(s => s.DeleteHabitAsync(It.IsAny<Guid>(), habitId, It.IsAny<CancellationToken>()))
                   .ReturnsAsync(true);

        var controller = CreateController(mockService);
        var result = await controller.DeleteHabit(habitId, CancellationToken.None);

        Assert.IsType<NoContentResult>(result);
    }

    /// <summary>
    /// Проверяет, что при попытке удалить несуществующую привычку возвращается 404 NotFound.
    /// </summary>
    [Fact]
    public async Task DeleteHabit_NotFound_ReturnsNotFound()
    {
        var habitId = Guid.NewGuid();

        var mockService = new Mock<IHabitService>();
        mockService.Setup(s => s.DeleteHabitAsync(It.IsAny<Guid>(), habitId, It.IsAny<CancellationToken>()))
                   .ReturnsAsync(false);

        var controller = CreateController(mockService);
        var result = await controller.DeleteHabit(habitId, CancellationToken.None);

        Assert.IsType<NotFoundObjectResult>(result);
    }
}
