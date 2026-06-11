using MediatR;
using HabitApi.Data;
using HabitApi.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace HabitApi.Features.Gamification;

public record GetGamificationQuery(Guid UserId) : IRequest<GamificationDto>;

public class GetGamificationHandler : IRequestHandler<GetGamificationQuery, GamificationDto>
{
    private readonly AppDbContext _db;

    private static readonly int[] LevelThresholds = [0, 100, 250, 500, 800, 1200, 1700, 2300, 3000, 3800, 4700, 5700, 6800, 8000, 9300];

    public GetGamificationHandler(AppDbContext db) => _db = db;

    public async Task<GamificationDto> Handle(GetGamificationQuery request, CancellationToken cancellationToken)
    {
        var level = await _db.UserLevels.FirstOrDefaultAsync(u => u.UserId == request.UserId, cancellationToken);
        if (level == null)
        {
            level = new Models.Domain.UserLevel { Id = Guid.NewGuid(), UserId = request.UserId, XP = 0, Level = 1, NextLevelXP = 100 };
            _db.UserLevels.Add(level);
            await _db.SaveChangesAsync(cancellationToken);
        }

        var achievements = await _db.Achievements
            .Where(a => a.UserId == request.UserId)
            .OrderByDescending(a => a.EarnedAt)
            .Take(5)
            .Select(a => new AchievementDto(a.Id, a.Type, a.Name, a.Description, a.Icon, a.EarnedAt))
            .ToListAsync(cancellationToken);

        var currentThreshold = level.Level < LevelThresholds.Length ? LevelThresholds[level.Level - 1] : 0;
        var nextThreshold = level.Level < LevelThresholds.Length ? LevelThresholds[level.Level] : currentThreshold + 1000;
        var progress = nextThreshold > currentThreshold
            ? (int)((level.XP - currentThreshold) * 100.0 / (nextThreshold - currentThreshold))
            : 100;

        return new GamificationDto(level.XP, level.Level, nextThreshold, progress, achievements);
    }
}
