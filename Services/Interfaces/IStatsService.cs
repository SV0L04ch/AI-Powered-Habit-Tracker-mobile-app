using HabitApi.Models.DTO;

namespace HabitApi.Services.Interfaces;

/// <summary>
/// Сервис для формирования статистики и сводок (персональных и городских).
/// </summary>
public interface IStatsService
{
    /// <summary>
    /// Получить ежедневную персональную сводку для пользователя.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя (из JWT).</param>
    /// <param name="date">Дата, за которую формируется сводка.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Сводка с выполненными привычками, погодой и ИИ-комментарием.</returns>
    Task<DailySummaryDto> GetDailySummaryAsync(Guid userId, DateOnly date, CancellationToken cancellationToken);

    /// <summary>
    /// Получить анонимную еженедельную сводку по городу.
    /// </summary>
    /// <param name="city">Название города.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Еженедельная сводка по городу.</returns>
    Task<CitySummaryDto> GetWeeklyCitySummaryAsync(string city, CancellationToken cancellationToken);
}
