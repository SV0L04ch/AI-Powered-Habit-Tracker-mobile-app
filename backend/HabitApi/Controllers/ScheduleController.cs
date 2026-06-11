using System.Security.Claims;
using HabitApi.Services;
using HabitApi.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace HabitApi.Controllers;

[ApiController]
[Route("api/schedule")]
public class ScheduleController : ControllerBase
{
    private readonly IScheduleService _scheduleService;

    public ScheduleController(IScheduleService scheduleService) => _scheduleService = scheduleService;

    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet("{habitId}")]
    public async Task<ActionResult<HabitScheduleDto>> GetSchedule(Guid habitId)
    {
        var schedule = await _scheduleService.GetScheduleAsync(GetUserId(), habitId);
        if (schedule == null) return NotFound();
        return Ok(new HabitScheduleDto(schedule.Id, schedule.HabitId, schedule.Frequency, schedule.DaysOfWeek, schedule.TimeOfDay, schedule.Exceptions));
    }

    [HttpPut("{habitId}")]
    public async Task<ActionResult<HabitScheduleDto>> UpsertSchedule(Guid habitId, [FromBody] UpsertScheduleRequest request)
    {
        var schedule = await _scheduleService.UpsertScheduleAsync(GetUserId(), habitId, request.Frequency, request.DaysOfWeek, request.TimeOfDay);
        return Ok(new HabitScheduleDto(schedule.Id, schedule.HabitId, schedule.Frequency, schedule.DaysOfWeek, schedule.TimeOfDay, schedule.Exceptions));
    }

    [HttpGet("today")]
    public async Task<ActionResult> GetTodayHabits()
    {
        var habits = await _scheduleService.GetTodayHabitsAsync(GetUserId());
        return Ok(habits.Select(h => new { h.Id, h.Name, h.TriggerType, h.TriggerValue }));
    }
}

public record UpsertScheduleRequest(string Frequency, List<int> DaysOfWeek, string? TimeOfDay);
