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
        // Проверка, что привычка принадлежит пользователю
        var habitExists = await _dbContext.Habits
            .AnyAsync(h => h.Id == habitId && h.UserId == userId, cancellationToken);
        if (!habitExists)
            return Array.Empty<HabitEntryDto>();

        var query = _dbContext.HabitEntries
            .Where(e => e.HabitId == habitId)
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
        // Загружаем привычку с проверкой прав
        var habit = await _dbContext.Habits
            .FirstOrDefaultAsync(h => h.Id == habitId && h.UserId == userId, cancellationToken);
        if (habit is null)
            throw new KeyNotFoundException("Habit not found for this user.");

        // Проверка на дубликат отметки за этот день
        var existingEntry = await _dbContext.HabitEntries
            .FirstOrDefaultAsync(e => e.HabitId == habitId && e.Date == request.Date, cancellationToken);
        if (existingEntry is not null)
            // Для клиента это конфликт данных, а не внутренняя ошибка сервера.
            throw new ConflictException("Entry for this habit and date already exists.");

        // Создание отметки в зависимости от типа привычки
        var entry = new HabitEntry
        {
            HabitId = habitId,
            Date = request.Date,
            Note = request.Note?.Trim()
        };

        if (habit.Type == HabitType.Positive)
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
        else // Negative habit
        {
            // Для отрицательных привычек используем RelapseCount, статус не нужен
            entry.RelapseCount = request.RelapseCount ?? 1; // по умолчанию 1, если не указано
        }

        _dbContext.HabitEntries.Add(entry);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return MapToDto(entry);
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
