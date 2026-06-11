using System.Security.Claims;
using HabitApi.Services;
using HabitApi.Services.Interfaces;
using HabitApi.Models.Domain;
using HabitApi.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HabitApi.Controllers;

[ApiController]
[Route("api/templates")]
[Authorize]
public class TemplatesController : ControllerBase
{
    private readonly ITemplateService _templateService;
    private readonly IHabitService _habitService;

    public TemplatesController(ITemplateService templateService, IHabitService habitService)
    {
        _templateService = templateService;
        _habitService = habitService;
    }

    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<ActionResult<List<HabitTemplateDto>>> GetTemplates([FromQuery] string? category = null)
    {
        var templates = await _templateService.GetTemplatesAsync(category);
        return Ok(templates);
    }

    [HttpPost("{templateId}/install")]
    public async Task<ActionResult<HabitDto>> InstallTemplate(Guid templateId)
    {
        var template = await _templateService.InstallTemplateAsync(GetUserId(), templateId);
        if (template == null) return NotFound("Template not found.");

        var createDto = new CreateHabitDto
        {
            Name = template.Name,
            IsPositive = template.IsPositive,
            HasPenalty = false,
            TriggerType = (TriggerType)template.TriggerType,
            TriggerValue = template.TriggerValue.ToString(),
            TargetDays = template.TargetDays,
            PenaltyDaysPerMiss = 0,
            Reminders = new List<string>()
        };

        var habit = await _habitService.CreateHabitAsync(GetUserId(), createDto, CancellationToken.None);
        return Ok(habit);
    }
}
