namespace HabitApi.Models.DTO;

/// <summary>
/// Ежедневная персональная сводка для пользователя.
/// </summary>
public sealed class DailySummaryDto
{
    /// <summary>
    /// Дата, за которую сформирована сводка.
    /// </summary>
    public DateOnly Date { get; set; }

    /// <summary>
    /// Количество полностью выполненных привычек.
    /// </summary>
    public int HabitsCompleted { get; set; }

    /// <summary>
    /// Количество частично выполненных привычек.
    /// </summary>
    public int HabitsPartiallyCompleted { get; set; }

    /// <summary>
    /// Количество пропущенных привычек.
    /// </summary>
    public int HabitsSkipped { get; set; }

    /// <summary>
    /// Снимок погоды на указанную дату в городе пользователя (может отсутствовать).
    /// </summary>
    public WeatherSnapshotDto? Weather { get; set; }

    /// <summary>
    /// Персональный комментарий от ИИ, основанный на статистике и погоде.
    /// </summary>
    public string AiInsight { get; set; } = string.Empty;
}

/// <summary>
/// Еженедельная анонимная сводка по городу.
/// </summary>
public sealed class CitySummaryDto
{
    /// <summary>
    /// Название города.
    /// </summary>
    public string City { get; set; } = string.Empty;

    /// <summary>
    /// Дата начала недели (понедельник).
    /// </summary>
    public DateOnly WeekStartDate { get; set; }

    /// <summary>
    /// Дата конца недели (воскресенье).
    /// </summary>
    public DateOnly WeekEndDate { get; set; }

    /// <summary>
    /// Список самых популярных привычек с количеством пользователей.
    /// </summary>
    public List<CityHabitStatDto> PopularHabits { get; set; } = new();
}

/// <summary>
/// Статистика по одной привычке в городской сводке.
/// </summary>
public sealed class CityHabitStatDto
{
    /// <summary>
    /// Название привычки.
    /// </summary>
    public string HabitName { get; set; } = string.Empty;

    /// <summary>
    /// Количество уникальных пользователей, выполнявших эту привычку.
    /// </summary>
    public int UserCount { get; set; }

    /// <summary>
    /// Общее количество пользователей в городе.
    /// </summary>
    public int TotalUsers { get; set; }

    /// <summary>
    /// Доля пользователей, выполнявших привычку (в процентах).
    /// </summary>
    public double Percentage => TotalUsers > 0 ? (double)UserCount / TotalUsers * 100 : 0;
}

/// <summary>
/// Снимок погоды для города на определённую дату.
/// </summary>
public sealed class WeatherSnapshotDto
{
    /// <summary>
    /// Название города.
    /// </summary>
    public string City { get; set; } = string.Empty;

    /// <summary>
    /// Дата, за которую получены данные.
    /// </summary>
    public DateOnly Date { get; set; }

    /// <summary>
    /// Текстовое описание погоды (например, "sunny", "rain", "clouds").
    /// </summary>
    public string Condition { get; set; } = "unknown";

    /// <summary>
    /// Температура в градусах Цельсия.
    /// </summary>
    public int TemperatureCelsius { get; set; }

    /// <summary>
    /// Влажность воздуха в процентах (если доступна).
    /// </summary>
    public int? HumidityPercent { get; set; }

    /// <summary>
    /// Тип осадков (например, "light rain", "snow"), если есть.
    /// </summary>
    public string? Precipitation { get; set; }
}
