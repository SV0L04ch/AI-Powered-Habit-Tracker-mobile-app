namespace HabitApi.Models.DTO;

/// <summary>
/// Ежедневная персональная сводка для пользователя.
/// </summary>
public sealed class DailySummaryDto
{
    public DateOnly Date { get; set; }
    public int HabitsCompleted { get; set; }
    public int HabitsPartiallyCompleted { get; set; }
    public int HabitsSkipped { get; set; }
    public WeatherSnapshotDto? Weather { get; set; }
    public string AiInsight { get; set; } = string.Empty;
}

/// <summary>
/// Еженедельная анонимная сводка по городу.
/// </summary>
public sealed class CitySummaryDto
{
    public string City { get; set; } = string.Empty;
    public DateOnly WeekStartDate { get; set; }
    public DateOnly WeekEndDate { get; set; }
    public List<CityHabitStatDto> PopularHabits { get; set; } = new();
}

/// <summary>
/// Статистика по одной привычке в городской сводке.
/// </summary>
public sealed class CityHabitStatDto
{
    public string HabitName { get; set; } = string.Empty;
    public int UserCount { get; set; }      // количество пользователей, выполнивших
    public int TotalUsers { get; set; }     // общее количество пользователей в городе (опционально)
    public double Percentage => TotalUsers > 0 ? (double)UserCount / TotalUsers * 100 : 0;
}

/// <summary>
/// Снимок погоды для города на дату.
/// </summary>
public sealed class WeatherSnapshotDto
{
    public string City { get; set; } = string.Empty;
    public DateOnly Date { get; set; }
    public string Condition { get; set; } = "unknown"; // "sunny", "rain", "clouds" и т.д.
    public int TemperatureCelsius { get; set; }
    public int? HumidityPercent { get; set; }
    public string? Precipitation { get; set; } // "light rain", "snow" и т.д.
}
