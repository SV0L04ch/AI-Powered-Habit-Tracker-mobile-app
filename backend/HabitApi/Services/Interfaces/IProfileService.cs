using HabitApi.Models.DTO;

namespace HabitApi.Services.Interfaces;

/// <summary>
/// Сервис для чтения и обновления профиля текущего пользователя.
/// </summary>
public interface IProfileService
{
    /// <summary>
    /// Получить профиль пользователя по идентификатору.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя (из JWT).</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>DTO профиля или null, если пользователь не найден.</returns>
    Task<UserProfileDto?> GetProfileAsync(Guid userId, CancellationToken cancellationToken);

    /// <summary>
    /// Обновить профиль пользователя. Изменяются только переданные поля.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя (из JWT).</param>
    /// <param name="request">Обновлённые данные профиля.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Обновлённый DTO профиля или null, если пользователь не найден.</returns>
    Task<UserProfileDto?> UpdateProfileAsync(Guid userId, UpdateUserProfileDto request, CancellationToken cancellationToken);
}
