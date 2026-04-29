using HabitApi.Data;
using HabitApi.Models.Domain;
using HabitApi.Models.DTO;
using HabitApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HabitApi.Services;

/// <summary>
/// Сервис для управления привычками.
/// </summary>
public sealed class HabitService : IHabitService
{
    private readonly AppDbContext _dbContext;

    public HabitService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<HabitDto>> GetUserHabitsAsync(Guid userId, CancellationToken cancellationToken)
    {
        var habits = await _dbContext.Habits
            .Where(h => h.UserId == userId && h.IsActive)
            .Select(h => MapToDto(h))
            .ToListAsync(cancellationToken);
        return habits;
    }

    /// <inheritdoc />
    public async Task<HabitDto?> GetHabitByIdAsync(Guid userId, Guid habitId, CancellationToken cancellationToken)
    {
        var habit = await _dbContext.Habits
            .FirstOrDefaultAsync(h => h.Id == habitId && h.UserId == userId && h.IsActive, cancellationToken);
        return habit is null ? null : MapToDto(habit);
    }

    /// <inheritdoc />
    public async Task<HabitDto> CreateHabitAsync(Guid userId, CreateHabitDto request, CancellationToken cancellationToken)
    {
        if (!request.HasPenalty && request.PenaltyDaysPerMiss != 0)
            throw new ArgumentException("PenaltyDaysPerMiss must be 0 for entertainment habits.");

        var habit = new Habit
        {
            UserId = userId,
            Name = request.Name.Trim(),
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

    /// <inheritdoc />
    public async Task<HabitDto?> UpdateHabitAsync(Guid userId, Guid habitId, UpdateHabitDto request, CancellationToken cancellationToken)
    {
        var habit = await _dbContext.Habits
            .FirstOrDefaultAsync(h => h.Id == habitId && h.UserId == userId && h.IsActive, cancellationToken);
        if (habit is null) return null;

        if (request.Name != null)
            habit.Name = request.Name.Trim();
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
        {
            if (habit.HasPenalty == false && request.PenaltyDaysPerMiss.Value != 0)
                throw new ArgumentException("PenaltyDaysPerMiss must be 0 for entertainment habits.");
            habit.PenaltyDaysPerMiss = request.PenaltyDaysPerMiss.Value;
        }
        if (request.Reminders != null)
            habit.Reminders = request.Reminders;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return MapToDto(habit);
    }

    /// <inheritdoc />
    public async Task<bool> DeleteHabitAsync(Guid userId, Guid habitId, CancellationToken cancellationToken)
    {
        var habit = await _dbContext.Habits
            .FirstOrDefaultAsync(h => h.Id == habitId && h.UserId == userId, cancellationToken);
        if (habit is null) return false;

        habit.IsActive = false;
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
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
