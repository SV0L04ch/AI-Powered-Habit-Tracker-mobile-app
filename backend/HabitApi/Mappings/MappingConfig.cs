using HabitApi.Models.Domain;
using HabitApi.Models.DTO;
using Mapster;

namespace HabitApi.Mappings;

public static class MappingConfig
{
    public static void Register()
    {
        TypeAdapterConfig<Habit, HabitDto>.NewConfig()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.IsPositive, src => src.IsPositive)
            .Map(dest => dest.HasPenalty, src => src.HasPenalty)
            .Map(dest => dest.TriggerType, src => src.TriggerType)
            .Map(dest => dest.TriggerValue, src => src.TriggerValue)
            .Map(dest => dest.TargetDays, src => src.TargetDays)
            .Map(dest => dest.PenaltyDaysPerMiss, src => src.PenaltyDaysPerMiss)
            .Map(dest => dest.Reminders, src => src.Reminders)
            .Map(dest => dest.IsActive, src => src.IsActive)
            .Map(dest => dest.CreatedAtUtc, src => src.CreatedAtUtc);

        TypeAdapterConfig<HabitTemplate, HabitTemplateDto>.NewConfig()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.Description, src => src.Description)
            .Map(dest => dest.Category, src => src.Category)
            .Map(dest => dest.Icon, src => src.Icon)
            .Map(dest => dest.IsPositive, src => src.IsPositive)
            .Map(dest => dest.InstallCount, src => src.InstallCount);

        TypeAdapterConfig<Quote, QuoteDto>.NewConfig()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Text, src => src.Text)
            .Map(dest => dest.Author, src => src.Author)
            .Map(dest => dest.Category, src => src.Category);
    }
}
