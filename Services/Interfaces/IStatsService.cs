using HabitApi.Models.DTO;

namespace HabitApi.Services.Interfaces;

public interface IStatsService
{
    Task<DailySummaryDto> GetDailySummaryAsync(Guid userId, DateOnly date, CancellationToken cancellationToken);
    Task<CitySummaryDto> GetWeeklyCitySummaryAsync(string city, CancellationToken cancellationToken);
    Task<HabitWeatherInsightResponseDto> GetHabitWeatherInsightAsync(
        Guid userId,
        Guid habitId,
        DateOnly date,
        bool includePreviousDayComparison,
        CancellationToken cancellationToken);
}
