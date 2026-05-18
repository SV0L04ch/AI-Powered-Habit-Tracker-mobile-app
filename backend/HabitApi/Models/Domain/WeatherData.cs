using System.ComponentModel.DataAnnotations;

namespace HabitApi.Models.Domain;

/// <summary>
/// Кэш погодных данных для города на определённую дату.
/// Используется для соблюдения НФТ: обновление не реже 1 раза в 3 часа.
/// </summary>
public sealed class WeatherData
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Название города.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string City { get; set; } = string.Empty;

    /// <summary>
    /// Дата, на которую получены данные.
    /// </summary>
    public DateOnly Date { get; set; }

    /// <summary>
    /// Текстовое описание погоды (солнечно, дождь, облачно и т.п.).
    /// </summary>
    [MaxLength(50)]
    public string Condition { get; set; } = string.Empty;

    /// <summary>
    /// Температура в градусах Цельсия.
    /// </summary>
    public int TemperatureCelsius { get; set; }

    /// <summary>
    /// Влажность воздуха в процентах.
    /// </summary>
    public int? HumidityPercent { get; set; }

    /// <summary>
    /// Тип осадков (лёгкий дождь, снег и т.п.).
    /// </summary>
    [MaxLength(100)]
    public string? Precipitation { get; set; }

    /// <summary>
    /// Время последнего обновления данных (UTC).
    /// </summary>
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
}
