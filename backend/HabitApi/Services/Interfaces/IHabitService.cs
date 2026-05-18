using HabitApi.Models.DTO;

namespace HabitApi.Services.Interfaces;

/// <summary>
/// Сервис для управления привычками пользователя.
/// </summary>
public interface IHabitService
{
    /// <summary>
    /// Получить все привычки пользователя.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя (из JWT).</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Коллекция DTO привычек.</returns>
    Task<IReadOnlyCollection<HabitDto>> GetUserHabitsAsync(Guid userId, CancellationToken cancellationToken);

    /// <summary>
    /// Получить привычку по идентификатору с проверкой принадлежности пользователю.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя (из JWT).</param>
    /// <param name="habitId">Идентификатор привычки.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>DTO привычки или null, если не найдена или не принадлежит пользователю.</returns>
    Task<HabitDto?> GetHabitByIdAsync(Guid userId, Guid habitId, CancellationToken cancellationToken);

    /// <summary>
    /// Создать новую привычку.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя (из JWT).</param>
    /// <param name="request">Данные для создания привычки.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Созданная привычка в виде DTO.</returns>
    Task<HabitDto> CreateHabitAsync(Guid userId, CreateHabitDto request, CancellationToken cancellationToken);

    /// <summary>
    /// Обновить существующую привычку.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя (из JWT).</param>
    /// <param name="habitId">Идентификатор привычки.</param>
    /// <param name="request">Обновлённые данные.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Обновлённая привычка или null, если не найдена.</returns>
    Task<HabitDto?> UpdateHabitAsync(Guid userId, Guid habitId, UpdateHabitDto request, CancellationToken cancellationToken);

    /// <summary>
    /// Удалить привычку (мягкое удаление).
    /// </summary>
    /// <param name="userId">Идентификатор пользователя (из JWT).</param>
    /// <param name="habitId">Идентификатор привычки.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>true, если удаление успешно, иначе false.</returns>
    Task<bool> DeleteHabitAsync(Guid userId, Guid habitId, CancellationToken cancellationToken);
}
