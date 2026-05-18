namespace HabitApi.Exceptions;

/// <summary>
/// Ошибка конфликта состояния данных.
/// Используется в случаях, когда запрос корректный,
/// но противоречит текущему состоянию системы
/// (например, пользователь с таким email уже существует).
/// </summary>
public sealed class ConflictException : Exception
{
    public ConflictException(string message) : base(message)
    {
    }

    // Новый конструктор
    public ConflictException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
