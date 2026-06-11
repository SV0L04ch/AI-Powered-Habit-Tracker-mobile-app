using HabitApi.Data;
using HabitApi.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace HabitApi.Services;

public interface ISocialService
{
    Task<List<SocialFeed>> GetCityFeedAsync(string city, int limit = 20);
    Task<SocialFeed> PostToFeedAsync(string city, string habitName);
    Task<List<Friendship>> GetFriendsAsync(Guid userId);
    Task<Friendship> SendFriendRequestAsync(Guid userId, Guid friendId);
    Task<Friendship?> AcceptFriendRequestAsync(Guid userId, Guid friendshipId);
    Task<List<Challenge>> GetChallengesAsync();
    Task<Challenge> CreateChallengeAsync(Guid userId, string name, string description, DateTime startDate, DateTime endDate);
    Task<ChallengeParticipant?> JoinChallengeAsync(Guid userId, Guid challengeId);
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

    public async Task<SocialFeed> PostToFeedAsync(string city, string habitName)
    {
        var feed = new SocialFeed
        {
            Id = Guid.NewGuid(),
            City = city,
            HabitName = habitName,
            CompletedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };
        _db.SocialFeed.Add(feed);
        await _db.SaveChangesAsync();
        return feed;
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

    public async Task<Friendship?> AcceptFriendRequestAsync(Guid userId, Guid friendshipId)
    {
        var friendship = await _db.Friendships
            .FirstOrDefaultAsync(f => f.Id == friendshipId && f.FriendId == userId && f.Status == "pending");
        if (friendship == null) return null;

        friendship.Status = "accepted";
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

    public async Task<ChallengeParticipant?> JoinChallengeAsync(Guid userId, Guid challengeId)
    {
        var challenge = await _db.Challenges.FindAsync(challengeId);
        if (challenge == null || !challenge.IsActive) return null;

        var alreadyJoined = await _db.ChallengeParticipants
            .AnyAsync(cp => cp.ChallengeId == challengeId && cp.UserId == userId);
        if (alreadyJoined) return null;

        var participant = new ChallengeParticipant
        {
            Id = Guid.NewGuid(),
            ChallengeId = challengeId,
            UserId = userId,
            CompletedCount = 0,
            JoinedAt = DateTime.UtcNow
        };
        _db.ChallengeParticipants.Add(participant);
        await _db.SaveChangesAsync();
        return participant;
    }
}
