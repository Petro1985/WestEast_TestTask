namespace DAL.Entity;

public class LoadHistory
{
    public Guid Id { get; set; }

    public DateTime CreatedAt { get; set; }

    /// <summary>
    ///     Откуда был загружен файл, например telegram или email
    /// </summary>
    public string Source { get; set; }

    /// <summary>
    ///     Хэш файл для проверки на дубли
    /// </summary>
    public string FileHash { get; set; }
}