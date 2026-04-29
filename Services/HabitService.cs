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
            .Include(h => h.HabitTags)
            .ThenInclude(ht => ht.Tag)
            .Where(h => h.UserId == userId && h.IsActive)
            .Select(h => MapToDto(h))
            .ToListAsync(cancellationToken);

        return habits;
    }

    /// <inheritdoc />
    public async Task<HabitDto?> GetHabitByIdAsync(Guid userId, Guid habitId, CancellationToken cancellationToken)
    {
        var habit = await _dbContext.Habits
            .Include(h => h.HabitTags)
            .ThenInclude(ht => ht.Tag)
            .FirstOrDefaultAsync(h => h.Id == habitId && h.UserId == userId && h.IsActive, cancellationToken);
        return habit is null ? null : MapToDto(habit);
    }

    /// <inheritdoc />
    public async Task<HabitDto> CreateHabitAsync(Guid userId, CreateHabitDto request, CancellationToken cancellationToken)
    {
        // Валидация: для развлекательных привычек PenaltyDaysPerMiss должен быть 0
        if (request.Category == HabitCategory.Entertainment && request.PenaltyDaysPerMiss != 0)
            throw new ArgumentException("PenaltyDaysPerMiss must be 0 for entertainment habits.");

        var habit = new Habit
        {
            UserId = userId,
            Name = request.Name.Trim(),
            Type = request.Type,
            Category = request.Category,
            TriggerType = request.TriggerType,
            TriggerValue = request.TriggerValue,
            TargetDays = request.TargetDays,
            PenaltyDaysPerMiss = request.PenaltyDaysPerMiss,
            Reminders = request.Reminders,
            CreatedAtUtc = DateTime.UtcNow,
            IsActive = true
        };

        // Добавление тегов
        if (request.TagIds.Any())
        {
            var tags = await _dbContext.Tags
                .Where(t => request.TagIds.Contains(t.Id) && t.UserId == userId)
                .ToListAsync(cancellationToken);
            foreach (var tag in tags)
            {
                habit.HabitTags.Add(new HabitTag { Tag = tag });
            }
        }

        _dbContext.Habits.Add(habit);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return MapToDto(habit);
    }

    /// <inheritdoc />
    public async Task<HabitDto?> UpdateHabitAsync(Guid userId, Guid habitId, UpdateHabitDto request, CancellationToken cancellationToken)
    {
        var habit = await _dbContext.Habits
            .Include(h => h.HabitTags)
            .FirstOrDefaultAsync(h => h.Id == habitId && h.UserId == userId && h.IsActive, cancellationToken);
        if (habit is null)
            return null;

        // Частичное обновление
        if (request.Name != null)
            habit.Name = request.Name.Trim();
        if (request.Type.HasValue)
            habit.Type = request.Type.Value;
        if (request.Category.HasValue)
            habit.Category = request.Category.Value;
        if (request.TriggerType.HasValue)
            habit.TriggerType = request.TriggerType.Value;
        if (request.TriggerValue != null)
            habit.TriggerValue = request.TriggerValue;
        if (request.TargetDays.HasValue)
            habit.TargetDays = request.TargetDays.Value;
        if (request.PenaltyDaysPerMiss.HasValue)
        {
            if (habit.Category == HabitCategory.Entertainment && request.PenaltyDaysPerMiss.Value != 0)
                throw new ArgumentException("PenaltyDaysPerMiss must be 0 for entertainment habits.");
            habit.PenaltyDaysPerMiss = request.PenaltyDaysPerMiss.Value;
        }
        if (request.Reminders != null)
            habit.Reminders = request.Reminders;

        // Обновление тегов (замена)
        if (request.TagIds != null)
        {
            habit.HabitTags.Clear();
            var tags = await _dbContext.Tags
                .Where(t => request.TagIds.Contains(t.Id) && t.UserId == userId)
                .ToListAsync(cancellationToken);
            foreach (var tag in tags)
            {
                habit.HabitTags.Add(new HabitTag { Tag = tag });
            }
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        return MapToDto(habit);
    }

    /// <inheritdoc />
    public async Task<bool> DeleteHabitAsync(Guid userId, Guid habitId, CancellationToken cancellationToken)
    {
        var habit = await _dbContext.Habits
            .FirstOrDefaultAsync(h => h.Id == habitId && h.UserId == userId, cancellationToken);
        if (habit is null)
            return false;

        // Мягкое удаление (сохраняем историю отметок)
        habit.IsActive = false;
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    /// <inheritdoc />
    public async Task<HabitDto?> AddTagAsync(Guid userId, Guid habitId, AddHabitTagDto request, CancellationToken cancellationToken)
    {
        var habit = await _dbContext.Habits
            .Include(h => h.HabitTags)
            .FirstOrDefaultAsync(h => h.Id == habitId && h.UserId == userId && h.IsActive, cancellationToken);
        if (habit is null)
            return null;

        var tag = await _dbContext.Tags
            .FirstOrDefaultAsync(t => t.Id == request.TagId && t.UserId == userId, cancellationToken);
        if (tag is null)
            return null;

        if (!habit.HabitTags.Any(ht => ht.TagId == tag.Id))
        {
            habit.HabitTags.Add(new HabitTag { Tag = tag });
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        return MapToDto(habit);
    }

    private static HabitDto MapToDto(Habit habit)
    {
        return new HabitDto
        {
            Id = habit.Id,
            Name = habit.Name,
            Type = habit.Type,
            Category = habit.Category,
            TriggerType = habit.TriggerType,
            TriggerValue = habit.TriggerValue,
            TargetDays = habit.TargetDays,
            PenaltyDaysPerMiss = habit.PenaltyDaysPerMiss,
            Reminders = habit.Reminders,
            Tags = habit.HabitTags
                .Where(ht => ht.Tag != null)
                .Select(ht => new TagDto { Id = ht.Tag!.Id, Name = ht.Tag!.Name })
                .ToList(),
            IsActive = habit.IsActive,
            CreatedAtUtc = habit.CreatedAtUtc
        };
    }
}
