using System.Security.Claims;
using HabitApi.Services;
using HabitApi.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HabitApi.Controllers;

[ApiController]
[Route("api/economics")]
[Authorize]
public class EconomicsController : ControllerBase
{
    private readonly IEconomicsService _economicsService;

    public EconomicsController(IEconomicsService economicsService) => _economicsService = economicsService;

    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet("wallet")]
    public async Task<ActionResult<WalletDto>> GetWallet()
    {
        var wallet = await _economicsService.GetWalletAsync(GetUserId());
        return Ok(wallet);
    }

    [HttpGet("transactions")]
    public async Task<ActionResult<List<TransactionDto>>> GetTransactions([FromQuery] int limit = 20)
    {
        var transactions = await _economicsService.GetTransactionsAsync(GetUserId(), limit);
        return Ok(transactions);
    }
}
