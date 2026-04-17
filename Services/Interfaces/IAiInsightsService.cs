using HabitApi.Models.DTO;

namespace HabitApi.Services.Interfaces;

/// <summary>
/// Сервис для интеграции с LLM (нейросетью) – генерация советов, сводок и аналитики.
/// </summary>
public interface IAiInsightsService
{
    /// <summary>
    /// Генерирует персональное текстовое сообщение для ежедневной сводки
    /// на основе статистики пользователя и погодных данных.
    /// </summary>
    /// <param name="summary">Сводка за день (выполненные привычки, погода).</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Текст с анализом и рекомендацией.</returns>
    Task<string> BuildDailyInsightAsync(DailySummaryDto summary, CancellationToken cancellationToken);

    /// <summary>
    /// Генерирует короткое поддерживающее сообщение для конкретной привычки
    /// в зависимости от сценария (лень, срыв, пропуск).
    /// </summary>
    /// <param name="habitName">Название привычки.</param>
    /// <param name="scenario">Сценарий: "lazy", "relapse", "skip".</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Текст с мотивацией или советом.</returns>
    Task<string> BuildHabitSupportMessageAsync(string habitName, string scenario, CancellationToken cancellationToken);

    /// <summary>
    /// Генерирует анонимную сводку по городу на основе агрегированных данных.
    /// </summary>
    /// <param name="city">Город.</param>
    /// <param name="stats">Агрегированные данные (список привычек с процентами выполнения).</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Текст сводки, например: "В Москве вчера 30% бегали до 10 утра..."</returns>
    Task<string> BuildCitySummaryAsync(string city, List<CityHabitStatDto> stats, CancellationToken cancellationToken);
}
