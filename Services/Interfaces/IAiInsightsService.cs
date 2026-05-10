using HabitApi.Models.DTO;

namespace HabitApi.Services.Interfaces;

public interface IAiInsightsService
{
    Task<string> BuildDailyInsightAsync(DailySummaryDto summary, CancellationToken cancellationToken);
    Task<string> BuildHabitSupportMessageAsync(string habitName, string scenario, CancellationToken cancellationToken);
    Task<string> BuildHabitWeatherInsightAsync(HabitWeatherInsightResponseDto summary, CancellationToken cancellationToken);
    Task<string> BuildCitySummaryAsync(string city, List<CityHabitStatDto> stats, CancellationToken cancellationToken);
}
