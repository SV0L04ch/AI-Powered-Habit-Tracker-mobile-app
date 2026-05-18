using System.ComponentModel.DataAnnotations;

namespace HabitApi.Models.DTO;

/// <summary>
/// Запрос на генерацию поддерживающего сообщения от ИИ.
/// </summary>
public sealed class HabitSupportRequestDto
{
    /// <summary>
    /// Сценарий запроса: "lazy" (лень), "relapse" (срыв), "skip" (пропуск).
    /// </summary>
    [Required]
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
