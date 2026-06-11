using System.Security.Claims;
using HabitApi.Services;
using HabitApi.Models.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HabitApi.Controllers;

[ApiController]
[Route("api/journal")]
[Authorize]
public class JournalController : ControllerBase
{
    private readonly IJournalService _journalService;
    public JournalController(IJournalService journalService) => _journalService = journalService;

    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet("notes/{habitId}")]
    public async Task<ActionResult> GetNotes(Guid habitId) => Ok(await _journalService.GetNotesAsync(GetUserId(), habitId));

    [HttpPost("notes/{habitId}")]
    public async Task<ActionResult> AddNote(Guid habitId, [FromBody] AddNoteRequest request) =>
        Ok(await _journalService.AddNoteAsync(GetUserId(), habitId, request.Text, request.Mood));

    [HttpGet("mood")]
    public async Task<ActionResult> GetMoodHistory([FromQuery] int days = 30) => Ok(await _journalService.GetMoodHistoryAsync(GetUserId(), days));

    [HttpPost("mood")]
    public async Task<ActionResult> LogMood([FromBody] LogMoodRequest request) =>
        Ok(await _journalService.LogMoodAsync(GetUserId(), request.Mood, request.Notes));

    [HttpGet("sleep")]
    public async Task<ActionResult> GetSleepHistory([FromQuery] int days = 30) => Ok(await _journalService.GetSleepHistoryAsync(GetUserId(), days));

    [HttpPost("sleep")]
    public async Task<ActionResult> LogSleep([FromBody] LogSleepRequest request) =>
        Ok(await _journalService.LogSleepAsync(GetUserId(), request.Bedtime, request.WakeTime, request.Quality, request.Notes));

    [HttpGet("meals")]
    public async Task<ActionResult> GetMealHistory([FromQuery] int days = 7) => Ok(await _journalService.GetMealHistoryAsync(GetUserId(), days));

    [HttpPost("meals")]
    public async Task<ActionResult> LogMeal([FromBody] LogMealRequest request) =>
        Ok(await _journalService.LogMealAsync(GetUserId(), request.Type, request.Foods, request.Calories, request.Notes));

    [HttpGet("goals")]
    public async Task<ActionResult> GetGoals() => Ok(await _journalService.GetGoalsAsync(GetUserId()));

    [HttpPost("goals")]
    public async Task<ActionResult> CreateGoal([FromBody] CreateGoalRequest request) =>
        Ok(await _journalService.CreateGoalAsync(GetUserId(), request.Title, request.TargetValue, request.Deadline));

    [HttpPut("goals/{goalId}")]
    public async Task<ActionResult> UpdateGoal(Guid goalId, [FromBody] UpdateGoalRequest request)
    {
        var result = await _journalService.UpdateGoalAsync(GetUserId(), goalId, request.CurrentValue, request.IsCompleted);
        if (result == null) return NotFound("Goal not found.");
        return Ok(result);
    }

    [HttpDelete("goals/{goalId}")]
    public async Task<ActionResult> DeleteGoal(Guid goalId)
    {
        var result = await _journalService.DeleteGoalAsync(GetUserId(), goalId);
        if (!result) return NotFound("Goal not found.");
        return NoContent();
    }
}

public record AddNoteRequest(string Text, int? Mood);
public record LogMoodRequest(int Mood, string? Notes);
public record LogSleepRequest(DateTime Bedtime, DateTime WakeTime, int Quality, string? Notes);
public record LogMealRequest(string Type, string Foods, int? Calories, string? Notes);
public record CreateGoalRequest(string Title, int TargetValue, DateTime Deadline);
public record UpdateGoalRequest(int? CurrentValue, bool? IsCompleted);
