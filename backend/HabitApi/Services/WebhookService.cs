using HabitApi.Data;
using HabitApi.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace HabitApi.Services;

public interface IWebhookService
{
    Task<List<Webhook>> GetUserWebhooksAsync(Guid userId);
    Task<Webhook> CreateWebhookAsync(Guid userId, string url, List<string> events, string? secret);
    Task<bool> DeleteWebhookAsync(Guid userId, Guid webhookId);
}

public class WebhookService : IWebhookService
{
    private readonly AppDbContext _db;

    public WebhookService(AppDbContext db) => _db = db;

    public async Task<List<Webhook>> GetUserWebhooksAsync(Guid userId)
    {
        return await _db.Webhooks
            .Where(w => w.UserId == userId)
            .OrderByDescending(w => w.CreatedAt)
            .ToListAsync();
    }

    public async Task<Webhook> CreateWebhookAsync(Guid userId, string url, List<string> events, string? secret)
    {
        var webhook = new Webhook
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Url = url,
            Events = events,
            Secret = secret,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        _db.Webhooks.Add(webhook);
        await _db.SaveChangesAsync();
        return webhook;
    }

    public async Task<bool> DeleteWebhookAsync(Guid userId, Guid webhookId)
    {
        var webhook = await _db.Webhooks.FirstOrDefaultAsync(w => w.Id == webhookId && w.UserId == userId);
        if (webhook == null) return false;

        _db.Webhooks.Remove(webhook);
        await _db.SaveChangesAsync();
        return true;
    }
}
