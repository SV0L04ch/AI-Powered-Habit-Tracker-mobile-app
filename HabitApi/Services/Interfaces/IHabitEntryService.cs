using HabitApi.Models.DTO;

namespace HabitApi.Services.Interfaces;

/// <summary>
/// Сервис для работы с отметками выполнения привычек.
/// </summary>
public interface IHabitEntryService
{
    /// <summary>
    /// Получить отметки выполнения для привычки за указанный период.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя (из JWT).</param>
    /// <param name="habitId">Идентификатор привычки.</param>
    /// <param name="fromDate">Начальная дата.</param>
    /// <param name="toDate">Конечная дата.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Коллекция DTO отметок.</returns>
    Task<IReadOnlyCollection<HabitEntryDto>> GetHabitEntriesAsync(
        Guid userId,
        Guid habitId,
        DateOnly? fromDate,
        DateOnly? toDate,
        CancellationToken cancellationToken);

    /// <summary>
    /// Добавить новую отметку выполнения для привычки.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя (из JWT).</param>
    /// <param name="habitId">Идентификатор привычки.</param>
    /// <param name="request">Данные отметки (дата, статус, количество и т.д.).</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Созданная отметка в виде DTO.</returns>
    /// <exception cref="KeyNotFoundException">Если привычка не найдена или не принадлежит пользователю.</exception>
    /// <exception cref="ArgumentException">Если данные не соответствуют типу привычки.</exception>
    Task<HabitEntryDto> AddHabitEntryAsync(
        Guid userId,
        Guid habitId,
        CreateHabitEntryDto request,
        CancellationToken cancellationToken);

    /// <summary>
    /// Обновить существующую отметку выполнения.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя (из JWT).</param>
    /// <param name="habitId">Идентификатор привычки.</param>
    /// <param name="entryId">Идентификатор отметки.</param>
    /// <param name="request">Новые данные отметки.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Обновлённая отметка или null, если привычка/отметка не найдена.</returns>
    Task<HabitEntryDto?> UpdateHabitEntryAsync(
        Guid userId,
        Guid habitId,
        Guid entryId,
        UpdateHabitEntryDto request,
        CancellationToken cancellationToken);

    /// <summary>
    /// Удалить существующую отметку выполнения.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя (из JWT).</param>
    /// <param name="habitId">Идентификатор привычки.</param>
    /// <param name="entryId">Идентификатор отметки.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>true, если удаление успешно; иначе false.</returns>
    Task<bool> DeleteHabitEntryAsync(
        Guid userId,
        Guid habitId,
        Guid entryId,
        CancellationToken cancellationToken);
}
