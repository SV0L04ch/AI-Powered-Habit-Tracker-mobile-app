using System.ComponentModel.DataAnnotations.Schema;

namespace HabitApi.Models.Domain;

/// <summary>
/// Связующая таблица для связи многие-ко-многим Habit - Tag.
/// </summary>
public sealed class HabitTag
{
    public Guid HabitId { get; set; }
    public Guid TagId { get; set; }

    [ForeignKey(nameof(HabitId))]
    public Habit? Habit { get; set; }

    [ForeignKey(nameof(TagId))]
    public Tag? Tag { get; set; }
}
