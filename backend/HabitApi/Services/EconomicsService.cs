using HabitApi.Data;
using HabitApi.Models.Domain;
using HabitApi.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace HabitApi.Services;

public interface IEconomicsService
{
    Task<WalletDto> GetWalletAsync(Guid userId);
    Task AddCurrencyAsync(Guid userId, int amount, string type, string description);
    Task<List<TransactionDto>> GetTransactionsAsync(Guid userId, int limit = 20);
}

public class EconomicsService : IEconomicsService
{
    private readonly AppDbContext _db;

    public EconomicsService(AppDbContext db) => _db = db;

    public async Task<WalletDto> GetWalletAsync(Guid userId)
    {
        var wallet = await _db.Wallets.FirstOrDefaultAsync(w => w.UserId == userId);
        if (wallet == null)
        {
            wallet = new Wallet { Id = Guid.NewGuid(), UserId = userId, Balance = 0, TotalEarned = 0 };
            _db.Wallets.Add(wallet);
            await _db.SaveChangesAsync();
        }
        return new WalletDto(wallet.Balance, wallet.TotalEarned);
    }

    public async Task AddCurrencyAsync(Guid userId, int amount, string type, string description)
    {
        var wallet = await _db.Wallets.FirstOrDefaultAsync(w => w.UserId == userId);
        if (wallet == null)
        {
            wallet = new Wallet { Id = Guid.NewGuid(), UserId = userId, Balance = 0, TotalEarned = 0 };
            _db.Wallets.Add(wallet);
        }

        wallet.Balance += amount;
        if (amount > 0) wallet.TotalEarned += amount;
        wallet.UpdatedAt = DateTime.UtcNow;

        _db.Transactions.Add(new Transaction
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Amount = amount,
            Type = type,
            Description = description,
            CreatedAt = DateTime.UtcNow
        });

        await _db.SaveChangesAsync();
    }

    public async Task<List<TransactionDto>> GetTransactionsAsync(Guid userId, int limit = 20)
    {
        return await _db.Transactions
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .Take(limit)
            .Select(t => new TransactionDto(t.Id, t.Amount, t.Type, t.Description, t.CreatedAt))
            .ToListAsync();
    }
}
