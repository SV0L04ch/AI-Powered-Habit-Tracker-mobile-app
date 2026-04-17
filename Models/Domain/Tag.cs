using System.ComponentModel.DataAnnotations;

namespace HabitApi.Models.Domain;

/// <summary>
/// Тег для группировки привычек.
/// </summary>
public sealed class Tag
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid UserId { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(7)] // цвет в формате #RRGGBB
    public string? Color { get; set; }

    // Навигационные свойства
    public User? User { get; set; }
    public ICollection<HabitTag> HabitTags { get; set; } = new List<HabitTag>();
}
