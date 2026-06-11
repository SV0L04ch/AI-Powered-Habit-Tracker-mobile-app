namespace HabitApi.Models.DTO;

public record HabitScheduleDto(
    Guid Id,
    Guid HabitId,
    string Frequency,
    List<int> DaysOfWeek,
    string? TimeOfDay,
    List<string> Exceptions
);

public record WalletDto(
    int Balance,
    int TotalEarned
);

public record TransactionDto(
    Guid Id,
    int Amount,
    string Type,
    string Description,
    DateTime CreatedAt
);
