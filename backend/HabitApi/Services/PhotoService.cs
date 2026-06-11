using HabitApi.Data;
using HabitApi.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace HabitApi.Services;

public interface IPhotoService
{
    Task<List<HabitPhoto>> GetPhotosByEntryAsync(Guid userId, Guid habitEntryId);
    Task<HabitPhoto> AddPhotoAsync(Guid userId, Guid habitEntryId, string photoUrl, string? caption);
    Task<bool> DeletePhotoAsync(Guid userId, Guid photoId);
}

public class PhotoService : IPhotoService
{
    private readonly AppDbContext _db;

    public PhotoService(AppDbContext db) => _db = db;

    public async Task<List<HabitPhoto>> GetPhotosByEntryAsync(Guid userId, Guid habitEntryId)
    {
        return await _db.HabitPhotos
            .Where(p => p.UserId == userId && p.HabitEntryId == habitEntryId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<HabitPhoto> AddPhotoAsync(Guid userId, Guid habitEntryId, string photoUrl, string? caption)
    {
        var photo = new HabitPhoto
        {
            Id = Guid.NewGuid(),
            HabitEntryId = habitEntryId,
            UserId = userId,
            PhotoUrl = photoUrl,
            Caption = caption,
            CreatedAt = DateTime.UtcNow
        };
        _db.HabitPhotos.Add(photo);
        await _db.SaveChangesAsync();
        return photo;
    }

    public async Task<bool> DeletePhotoAsync(Guid userId, Guid photoId)
    {
        var photo = await _db.HabitPhotos.FirstOrDefaultAsync(p => p.Id == photoId && p.UserId == userId);
        if (photo == null) return false;

        _db.HabitPhotos.Remove(photo);
        await _db.SaveChangesAsync();
        return true;
    }
}
