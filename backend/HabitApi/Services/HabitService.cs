using HabitApi.Data;
using HabitApi.Models.Domain;
using HabitApi.Models.DTO;
using HabitApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HabitApi.Services;

/// <summary>
/// Сервис для управления привычками пользователя.
/// Обеспечивает создание, чтение, обновление и мягкое удаление привычек.
/// </summary>
public sealed class HabitService : IHabitService
{
    private readonly AppDbContext _dbContext;

    /// <summary>
    /// Инициализирует сервис привычек с контекстом базы данных.
    /// </summary>
    /// <param name="dbContext">Контекст базы данных приложения.</param>
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
        // Проверка валидности TriggerValue при создании (пункт 10 аудита)
        if (!IsValidTriggerValue(request.TriggerType, request.TriggerValue))
            throw new ArgumentException("TriggerValue is invalid for the selected TriggerType.");

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

    /// <inheritdoc />
    public async Task<HabitDto?> UpdateHabitAsync(Guid userId, Guid habitId, UpdateHabitDto request, CancellationToken cancellationToken)
    {
        // Ищем привычку без фильтра IsActive, чтобы можно было повторно активировать
        var habit = await _dbContext.Habits
            .FirstOrDefaultAsync(h => h.Id == habitId && h.UserId == userId, cancellationToken);
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
        // Новая проверка – теперь IsActive обновляется
        if (request.IsActive.HasValue)
            habit.IsActive = request.IsActive.Value;

        // Проверка корректности TriggerValue после обновления
        if (!string.IsNullOrEmpty(habit.TriggerValue) && !IsValidTriggerValue(habit.TriggerType, habit.TriggerValue))
            throw new ArgumentException("TriggerValue is invalid for the selected TriggerType.");

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

    /// <summary>
    /// Проверяет, соответствует ли значение триггера его типу.
    /// </summary>
    /// <param name="type">Тип триггера (TimeOfDay или CountPerDay).</param>
    /// <param name="value">Строковое значение (время или количество).</param>
    /// <returns>true, если значение валидно для указанного типа.</returns>
    private static bool IsValidTriggerValue(TriggerType type, string value)
    {
        if (string.IsNullOrEmpty(value)) return false;
        if (type == TriggerType.CountPerDay)
            return int.TryParse(value, out var count) && count > 0;
        if (type == TriggerType.TimeOfDay)
            return TimeSpan.TryParseExact(value, @"hh\:mm", null, out _);
        return false;
    }

    /// <summary>
    /// Преобразует сущность Habit в DTO для передачи клиенту.
    /// </summary>
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
