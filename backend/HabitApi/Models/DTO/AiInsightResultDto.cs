namespace HabitApi.Models.DTO;

public sealed class AiInsightResultDto
{
    public string Message { get; set; } = string.Empty;

    public bool IsFallback { get; set; }

    public string? FallbackReason { get; set; }
}
