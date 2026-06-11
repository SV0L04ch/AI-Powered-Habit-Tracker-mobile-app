using HabitApi.Data;
using HabitApi.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace HabitApi.Services;

public interface ISocialService
{
    Task<List<SocialFeed>> GetCityFeedAsync(string city, int limit = 20);
    Task PostToFeedAsync(string city, string habitName);
    Task<List<Friendship>> GetFriendsAsync(Guid userId);
    Task<Friendship> SendFriendRequestAsync(Guid userId, Guid friendId);
    Task<List<Challenge>> GetChallengesAsync();
    Task<Challenge> CreateChallengeAsync(Guid userId, string name, string description, DateTime startDate, DateTime endDate);
}

public class SocialService : ISocialService
{
    private readonly AppDbContext _db;
    public SocialService(AppDbContext db) => _db = db;

    public async Task<List<SocialFeed>> GetCityFeedAsync(string city, int limit = 20)
    {
        return await _db.SocialFeed
            .Where(f => f.City == city)
            .OrderByDescending(f => f.CompletedAt)
            .Take(limit)
            .ToListAsync();
    }

    public async Task PostToFeedAsync(string city, string habitName)
    {
        _db.SocialFeed.Add(new SocialFeed
        {
            Id = Guid.NewGuid(),
            City = city,
            HabitName = habitName,
            CompletedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        });
        await _db.SaveChangesAsync();
    }

    public async Task<List<Friendship>> GetFriendsAsync(Guid userId)
    {
        return await _db.Friendships
            .Where(f => (f.UserId == userId || f.FriendId == userId) && f.Status == "accepted")
            .ToListAsync();
    }

    public async Task<Friendship> SendFriendRequestAsync(Guid userId, Guid friendId)
    {
        var friendship = new Friendship
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            FriendId = friendId,
            Status = "pending",
            CreatedAt = DateTime.UtcNow
        };
        _db.Friendships.Add(friendship);
        await _db.SaveChangesAsync();
        return friendship;
    }

    public async Task<List<Challenge>> GetChallengesAsync()
    {
        return await _db.Challenges.Where(c => c.IsActive).OrderByDescending(c => c.CreatedAt).ToListAsync();
    }

    public async Task<Challenge> CreateChallengeAsync(Guid userId, string name, string description, DateTime startDate, DateTime endDate)
    {
        var challenge = new Challenge
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = description,
            CreatorId = userId,
            StartDate = startDate,
            EndDate = endDate,
            CreatedAt = DateTime.UtcNow
        };
        _db.Challenges.Add(challenge);
        await _db.SaveChangesAsync();
        return challenge;
    }
}
