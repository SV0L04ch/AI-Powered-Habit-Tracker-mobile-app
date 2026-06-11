using System.Security.Claims;
using HabitApi.Services;
using HabitApi.Models.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HabitApi.Controllers;

[ApiController]
[Route("api/webhooks")]
[Authorize]
public class WebhooksController : ControllerBase
{
    private readonly IWebhookService _webhookService;

    public WebhooksController(IWebhookService webhookService) => _webhookService = webhookService;

    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<ActionResult<List<Webhook>>> GetWebhooks() =>
        Ok(await _webhookService.GetUserWebhooksAsync(GetUserId()));

    [HttpPost]
    public async Task<ActionResult<Webhook>> CreateWebhook([FromBody] CreateWebhookRequest request)
    {
        var webhook = await _webhookService.CreateWebhookAsync(GetUserId(), request.Url, request.Events, request.Secret);
        return Ok(webhook);
    }

    [HttpDelete("{webhookId}")]
    public async Task<ActionResult> DeleteWebhook(Guid webhookId)
    {
        var result = await _webhookService.DeleteWebhookAsync(GetUserId(), webhookId);
        if (!result) return NotFound("Webhook not found.");
        return NoContent();
    }
}

public record CreateWebhookRequest(string Url, List<string> Events, string? Secret);
