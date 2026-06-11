using HabitApi.Data;
using HabitApi.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace HabitApi.Services;

public interface ILeagueService
{
    Task<List<League>> GetLeaguesAsync();
    Task<League?> GetUserLeagueAsync(Guid userId);
}

public class LeagueService : ILeagueService
{
    private readonly AppDbContext _db;

    public LeagueService(AppDbContext db) => _db = db;

    public async Task<List<League>> GetLeaguesAsync()
    {
        return await _db.Leagues.OrderBy(l => l.MinXP).ToListAsync();
    }

    public async Task<League?> GetUserLeagueAsync(Guid userId)
    {
        var level = await _db.UserLevels.FirstOrDefaultAsync(u => u.UserId == userId);
        if (level == null) return null;

        return await _db.Leagues
            .Where(l => level.XP >= l.MinXP && level.XP <= l.MaxXP)
            .OrderByDescending(l => l.MinXP)
            .FirstOrDefaultAsync();
    }
}
