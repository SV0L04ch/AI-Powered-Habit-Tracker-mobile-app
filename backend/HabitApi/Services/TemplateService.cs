using HabitApi.Data;
using HabitApi.Models.Domain;
using HabitApi.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace HabitApi.Services;

public interface ITemplateService
{
    Task<List<HabitTemplateDto>> GetTemplatesAsync(string? category = null);
    Task<HabitTemplate?> InstallTemplateAsync(Guid userId, Guid templateId);
}

public class TemplateService : ITemplateService
{
    private readonly AppDbContext _db;

    public TemplateService(AppDbContext db) => _db = db;

    public async Task<List<HabitTemplateDto>> GetTemplatesAsync(string? category = null)
    {
        var query = _db.HabitTemplates.Where(t => t.IsActive);

        if (!string.IsNullOrEmpty(category))
            query = query.Where(t => t.Category == category);

        return await query
            .OrderByDescending(t => t.InstallCount)
            .Select(t => new HabitTemplateDto(t.Id, t.Name, t.Description, t.Category, t.Icon, t.IsPositive, t.InstallCount))
            .ToListAsync();
    }

    public async Task<HabitTemplate?> InstallTemplateAsync(Guid userId, Guid templateId)
    {
        var template = await _db.HabitTemplates.FindAsync(templateId);
        if (template == null) return null;

        template.InstallCount++;
        await _db.SaveChangesAsync();
        return template;
    }
}
