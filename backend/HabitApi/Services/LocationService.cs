using HabitApi.Data;
using HabitApi.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace HabitApi.Services;

public interface ILocationService
{
    Task<List<HabitLocation>> GetLocationsByEntryAsync(Guid userId, Guid habitEntryId);
    Task<HabitLocation> AddLocationAsync(Guid userId, Guid habitEntryId, double latitude, double longitude, string? name);
    Task<bool> DeleteLocationAsync(Guid userId, Guid locationId);
}

public class LocationService : ILocationService
{
    private readonly AppDbContext _db;

    public LocationService(AppDbContext db) => _db = db;

    public async Task<List<HabitLocation>> GetLocationsByEntryAsync(Guid userId, Guid habitEntryId)
    {
        return await _db.HabitLocations
            .Where(l => l.UserId == userId && l.HabitEntryId == habitEntryId)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync();
    }

    public async Task<HabitLocation> AddLocationAsync(Guid userId, Guid habitEntryId, double latitude, double longitude, string? name)
    {
        var location = new HabitLocation
        {
            Id = Guid.NewGuid(),
            HabitEntryId = habitEntryId,
            UserId = userId,
            Latitude = latitude,
            Longitude = longitude,
            Name = name,
            CreatedAt = DateTime.UtcNow
        };
        _db.HabitLocations.Add(location);
        await _db.SaveChangesAsync();
        return location;
    }

    public async Task<bool> DeleteLocationAsync(Guid userId, Guid locationId)
    {
        var location = await _db.HabitLocations.FirstOrDefaultAsync(l => l.Id == locationId && l.UserId == userId);
        if (location == null) return false;

        _db.HabitLocations.Remove(location);
        await _db.SaveChangesAsync();
        return true;
    }
}
