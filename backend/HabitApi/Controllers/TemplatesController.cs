using HabitApi.Services;
using HabitApi.Models.Domain;
using HabitApi.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace HabitApi.Controllers;

[ApiController]
[Route("api/templates")]
public class TemplatesController : ControllerBase
{
    private readonly ITemplateService _templateService;

    public TemplatesController(ITemplateService templateService) => _templateService = templateService;

    [HttpGet]
    public async Task<ActionResult<List<HabitTemplateDto>>> GetTemplates([FromQuery] string? category = null)
    {
        var templates = await _templateService.GetTemplatesAsync(category);
        return Ok(templates);
    }
}
