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
    /// <param name="fromDate">Начальная дата (опционально).</param>
    /// <param name="toDate">Конечная дата (опционально).</param>
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
}
