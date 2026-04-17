using System.ComponentModel.DataAnnotations;

namespace HabitApi.Models.DTO;

/// <summary>
/// DTO для добавления существующего тега к привычке.
/// </summary>
public sealed class AddHabitTagDto
{
    /// <summary>
    /// Идентификатор тега.
    /// </summary>
    [Required]
    public Guid TagId { get; set; }
}
