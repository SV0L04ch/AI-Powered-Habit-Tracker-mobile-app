using HabitApi.Models.Domain;

namespace HabitApi.Models.DTO;

/// <summary>
/// Запрос на генерацию поддерживающего сообщения от ИИ.
/// </summary>
public sealed class HabitSupportRequestDto
{
    /// <summary>
    /// Сценарий запроса: "lazy" (лень), "relapse" (срыв), "skip" (пропуск).
    /// </summary>
    public string Scenario { get; set; } = "lazy";
}

/// <summary>
/// Ответ с поддерживающим сообщением от ИИ.
/// </summary>
public sealed class HabitSupportResponseDto
{
    /// <summary>
    /// Идентификатор привычки, для которой сгенерировано сообщение.
    /// </summary>
    public Guid HabitId { get; set; }

    /// <summary>
    /// Сценарий запроса (возвращается как эхо для ясности).
    /// </summary>
    public string Scenario { get; set; } = string.Empty;

    /// <summary>
    /// Текст сгенерированного сообщения (мотивация, совет).
    /// </summary>
    public string Message { get; set; } = string.Empty;
}

public sealed class HabitWeatherInsightRequestDto
{
    public DateOnly? Date { get; set; }
    public bool IncludePreviousDayComparison { get; set; } = true;
}

public sealed class HabitWeatherInsightResponseDto
{
    public Guid HabitId { get; set; }
    public string HabitName { get; set; } = string.Empty;
    public bool IsPositive { get; set; }
    public DateOnly Date { get; set; }
    public HabitWeatherDaySummaryDto CurrentDay { get; set; } = new();
    public HabitWeatherDaySummaryDto? PreviousDay { get; set; }
    public string Message { get; set; } = string.Empty;
}

public sealed class HabitWeatherDaySummaryDto
{
    public DateOnly Date { get; set; }
    public bool HasEntry { get; set; }
    public HabitEntryStatus? Status { get; set; }
    public int? PartialValue { get; set; }
    public int? RelapseCount { get; set; }
    public string? Note { get; set; }
    public WeatherSnapshotDto? Weather { get; set; }
    public string PerformanceSummary { get; set; } = string.Empty;
}
