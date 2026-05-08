using HabitApi.Data;
using HabitApi.Models.Domain;
using HabitApi.Models.DTO;
using HabitApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HabitApi.Services;

public sealed class HabitService : IHabitService
{
    private readonly AppDbContext _dbContext;

    public HabitService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<HabitDto>> GetUserHabitsAsync(Guid userId, CancellationToken cancellationToken)
    {
        var habits = await _dbContext.Habits
            .Where(h => h.UserId == userId && h.IsActive)
            .Select(h => MapToDto(h))
            .ToListAsync(cancellationToken);
        return habits;
    }

    public async Task<HabitDto?> GetHabitByIdAsync(Guid userId, Guid habitId, CancellationToken cancellationToken)
    {
        var habit = await _dbContext.Habits
            .FirstOrDefaultAsync(h => h.Id == habitId && h.UserId == userId && h.IsActive, cancellationToken);
        return habit is null ? null : MapToDto(habit);
    }

    public async Task<HabitDto> CreateHabitAsync(Guid userId, CreateHabitDto request, CancellationToken cancellationToken)
    {
        var habit = new Habit
        {
            UserId = userId,
            Name = request.Name.Trim().Normalize(),
            IsPositive = request.IsPositive,
            HasPenalty = request.HasPenalty,
            TriggerType = request.TriggerType,
            TriggerValue = request.TriggerValue,
            TargetDays = request.TargetDays,
            PenaltyDaysPerMiss = request.PenaltyDaysPerMiss,
            Reminders = request.Reminders,
            CreatedAtUtc = DateTime.UtcNow,
            IsActive = true
        };

        _dbContext.Habits.Add(habit);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return MapToDto(habit);
    }

    public async Task<HabitDto?> UpdateHabitAsync(Guid userId, Guid habitId, UpdateHabitDto request, CancellationToken cancellationToken)
    {
        var habit = await _dbContext.Habits
            .FirstOrDefaultAsync(h => h.Id == habitId && h.UserId == userId && h.IsActive, cancellationToken);
        if (habit is null) return null;

        if (request.Name != null)
            habit.Name = request.Name.Trim().Normalize();
        if (request.IsPositive.HasValue)
            habit.IsPositive = request.IsPositive.Value;
        if (request.HasPenalty.HasValue)
            habit.HasPenalty = request.HasPenalty.Value;
        if (request.TriggerType.HasValue)
            habit.TriggerType = request.TriggerType.Value;
        if (request.TriggerValue != null)
            habit.TriggerValue = request.TriggerValue;
        if (request.TargetDays.HasValue)
            habit.TargetDays = request.TargetDays.Value;
        if (request.PenaltyDaysPerMiss.HasValue)
            habit.PenaltyDaysPerMiss = request.PenaltyDaysPerMiss.Value;
        if (request.Reminders != null)
            habit.Reminders = request.Reminders;

        // Проверка консистентности после обновления
        if (!string.IsNullOrEmpty(habit.TriggerValue) && !IsValidTriggerValue(habit.TriggerType, habit.TriggerValue))
            throw new ArgumentException("TriggerValue is invalid for the selected TriggerType.");

        await _dbContext.SaveChangesAsync(cancellationToken);
        return MapToDto(habit);
    }

    public async Task<bool> DeleteHabitAsync(Guid userId, Guid habitId, CancellationToken cancellationToken)
    {
        var habit = await _dbContext.Habits
            .FirstOrDefaultAsync(h => h.Id == habitId && h.UserId == userId, cancellationToken);
        if (habit is null) return false;

        habit.IsActive = false;
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static bool IsValidTriggerValue(TriggerType type, string value)
    {
        if (string.IsNullOrEmpty(value)) return false;
        if (type == TriggerType.CountPerDay)
            return int.TryParse(value, out int count) && count > 0;
        if (type == TriggerType.TimeOfDay)
            return TimeSpan.TryParseExact(value, @"hh\:mm", null, out _);
        return false;
    }

    private static HabitDto MapToDto(Habit habit)
    {
        return new HabitDto
        {
            Id = habit.Id,
            Name = habit.Name,
            IsPositive = habit.IsPositive,
            HasPenalty = habit.HasPenalty,
            TriggerType = habit.TriggerType,
            TriggerValue = habit.TriggerValue,
            TargetDays = habit.TargetDays,
            PenaltyDaysPerMiss = habit.PenaltyDaysPerMiss,
            Reminders = habit.Reminders,
            IsActive = habit.IsActive,
            CreatedAtUtc = habit.CreatedAtUtc
        };
    }
}
