using MediatR;
using HabitApi.Data;
using HabitApi.Models.DTO;
using Microsoft.EntityFrameworkCore;
using Mapster;

namespace HabitApi.Features.Streaks;

public record GetStreaksQuery(Guid UserId) : IRequest<List<StreakDto>>;

public class GetStreaksHandler : IRequestHandler<GetStreaksQuery, List<StreakDto>>
{
    private readonly AppDbContext _db;

    public GetStreaksHandler(AppDbContext db) => _db = db;

    public async Task<List<StreakDto>> Handle(GetStreaksQuery request, CancellationToken cancellationToken)
    {
        return await _db.Streaks
            .Where(s => s.UserId == request.UserId)
            .Join(_db.Habits, s => s.HabitId, h => h.Id, (s, h) => new StreakDto(
                s.Id, s.HabitId, h.Name, s.CurrentStreak, s.LongestStreak, s.LastCompletedDate
            ))
            .OrderByDescending(s => s.CurrentStreak)
            .ToListAsync(cancellationToken);
    }
}
