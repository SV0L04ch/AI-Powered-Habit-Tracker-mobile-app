namespace HabitApi.Models.Domain;

public class Wallet
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public int Balance { get; set; }
    public int TotalEarned { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public class Transaction
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public int Amount { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
