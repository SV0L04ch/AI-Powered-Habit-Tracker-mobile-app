using HabitApi.Data;
using HabitApi.Exceptions;
using HabitApi.Models.Domain;
using HabitApi.Models.DTO;
using HabitApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HabitApi.Services;

/// <summary>
/// Сервис для работы с отметками выполнения привычек.
/// </summary>
public sealed class HabitEntryService : IHabitEntryService
{
    private readonly AppDbContext _dbContext;

    public HabitEntryService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<HabitEntryDto>> GetHabitEntriesAsync(
        Guid userId,
        Guid habitId,
        DateOnly? fromDate,
        DateOnly? toDate,
        CancellationToken cancellationToken)
    {
        var habit = await GetOwnedActiveHabitAsync(userId, habitId, cancellationToken);
        if (habit is null)
            return Array.Empty<HabitEntryDto>();

        var query = _dbContext.HabitEntries
            .Where(e => e.HabitId == habit.Id)
            .AsQueryable();

        if (fromDate.HasValue)
            query = query.Where(e => e.Date >= fromDate.Value);
        if (toDate.HasValue)
            query = query.Where(e => e.Date <= toDate.Value);

        var entries = await query
            .OrderByDescending(e => e.Date)
            .Select(e => MapToDto(e))
            .ToListAsync(cancellationToken);

        return entries;
    }

    /// <inheritdoc />
    public async Task<HabitEntryDto> AddHabitEntryAsync(
        Guid userId,
        Guid habitId,
        CreateHabitEntryDto request,
        CancellationToken cancellationToken)
    {
        var habit = await GetOwnedActiveHabitAsync(userId, habitId, cancellationToken);
        if (habit is null)
            throw new KeyNotFoundException("Habit not found for this user.");

        var existingEntry = await _dbContext.HabitEntries
            .FirstOrDefaultAsync(e => e.HabitId == habitId && e.Date == request.Date, cancellationToken);
        if (existingEntry is not null)
            throw new ConflictException("Entry for this habit and date already exists.");

        var entry = new HabitEntry
        {
            HabitId = habitId,
            Date = request.Date,
            Note = string.IsNullOrWhiteSpace(request.Note) ? null : request.Note.Trim()
        };
        
        if (habit.IsPositive)
        {
            if (request.Status is null)
                throw new ArgumentException("Status is required for positive habits.");
            entry.Status = request.Status;

            if (request.Status == HabitEntryStatus.Partial)
            {
                if (!request.PartialValue.HasValue)
                    throw new ArgumentException("PartialValue is required when status is Partial.");
                entry.PartialValue = request.PartialValue;
            }
        }
        else // Отрицательная привычка
        {
            entry.RelapseCount = request.RelapseCount ?? 1; // по умолчанию 1 срыв
        }

        _dbContext.HabitEntries.Add(entry);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return MapToDto(entry);
    }

    /// <inheritdoc />
    public async Task<HabitEntryDto?> UpdateHabitEntryAsync(
        Guid userId,
        Guid habitId,
        Guid entryId,
        UpdateHabitEntryDto request,
        CancellationToken cancellationToken)
    {
        var habit = await GetOwnedActiveHabitAsync(userId, habitId, cancellationToken);
        if (habit is null)
            return null;

        var entry = await _dbContext.HabitEntries
            .FirstOrDefaultAsync(e => e.Id == entryId && e.HabitId == habit.Id, cancellationToken);
        if (entry is null)
            return null;

        var targetDate = request.Date ?? entry.Date;
        var duplicateEntryExists = await _dbContext.HabitEntries
            .AnyAsync(
                e => e.HabitId == habit.Id
                    && e.Id != entry.Id
                    && e.Date == targetDate,
                cancellationToken);
        if (duplicateEntryExists)
            throw new ConflictException("Entry for this habit and date already exists.");

        entry.Date = targetDate;

        // Пустая строка очищает заметку, а null означает «поле не меняем».
        if (request.Note is not null)
            entry.Note = string.IsNullOrWhiteSpace(request.Note) ? null : request.Note.Trim();

        ApplyEntryValuesByHabitType(habit, entry, request);

        await _dbContext.SaveChangesAsync(cancellationToken);
        return MapToDto(entry);
    }

    /// <inheritdoc />
    public async Task<bool> DeleteHabitEntryAsync(
        Guid userId,
        Guid habitId,
        Guid entryId,
        CancellationToken cancellationToken)
    {
        var habit = await GetOwnedActiveHabitAsync(userId, habitId, cancellationToken);
        if (habit is null)
            return false;

        var entry = await _dbContext.HabitEntries
            .FirstOrDefaultAsync(e => e.Id == entryId && e.HabitId == habit.Id, cancellationToken);
        if (entry is null)
            return false;

        _dbContext.HabitEntries.Remove(entry);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static void ApplyEntryValuesByHabitType(Habit habit, HabitEntry entry, UpdateHabitEntryDto request)
    {
        if (habit.IsPositive)
        {
            var targetStatus = request.Status ?? entry.Status;
            if (targetStatus is null)
                throw new ArgumentException("Status is required for positive habits.");

            entry.Status = targetStatus;
            entry.RelapseCount = null;

            if (targetStatus == HabitEntryStatus.Partial)
            {
                var targetPartialValue = request.PartialValue ?? entry.PartialValue;
                if (!targetPartialValue.HasValue)
                    throw new ArgumentException("PartialValue is required when status is Partial.");

                entry.PartialValue = targetPartialValue.Value;
            }
            else
            {
                // Для Completed/Skipped частичное значение хранить не нужно.
                entry.PartialValue = null;
            }

            return;
        }

        entry.Status = null;
        entry.PartialValue = null;
        entry.RelapseCount = request.RelapseCount ?? entry.RelapseCount ?? 1;
    }

    private async Task<Habit?> GetOwnedActiveHabitAsync(Guid userId, Guid habitId, CancellationToken cancellationToken)
    {
        return await _dbContext.Habits
            .FirstOrDefaultAsync(
                h => h.Id == habitId
                    && h.UserId == userId
                    && h.IsActive,
                cancellationToken);
    }

    private static HabitEntryDto MapToDto(HabitEntry entry)
    {
        return new HabitEntryDto
        {
            Id = entry.Id,
            HabitId = entry.HabitId,
            Date = entry.Date,
            Status = entry.Status,
            PartialValue = entry.PartialValue,
            RelapseCount = entry.RelapseCount,
            Note = entry.Note,
            CreatedAtUtc = entry.CreatedAtUtc
        };
    }
}
