using HabitApi.Data;
using HabitApi.Models.Domain;
using HabitApi.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace HabitApi.Services;

public interface IQuoteService
{
    Task<QuoteDto?> GetDailyQuoteAsync();
}

public class QuoteService : IQuoteService
{
    private readonly AppDbContext _db;

    public QuoteService(AppDbContext db) => _db = db;

    public async Task<QuoteDto?> GetDailyQuoteAsync()
    {
        var today = DateTime.UtcNow.Date;
        var dayOfYear = today.DayOfYear;

        var quote = await _db.Quotes
            .Where(q => q.IsActive)
            .Skip(dayOfYear % await _db.Quotes.CountAsync(q => q.IsActive))
            .FirstOrDefaultAsync();

        if (quote == null) return null;

        return new QuoteDto(quote.Id, quote.Text, quote.Author, quote.Category);
    }
}
