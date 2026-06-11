using HabitApi.Services;
using HabitApi.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace HabitApi.Controllers;

[ApiController]
[Route("api/quotes")]
public class QuotesController : ControllerBase
{
    private readonly IQuoteService _quoteService;

    public QuotesController(IQuoteService quoteService) => _quoteService = quoteService;

    [HttpGet("daily")]
    public async Task<ActionResult<QuoteDto>> GetDailyQuote()
    {
        var quote = await _quoteService.GetDailyQuoteAsync();
        if (quote == null) return NotFound();
        return Ok(quote);
    }
}
