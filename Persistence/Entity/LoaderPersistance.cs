namespace DAL.Entity;

public class LoadHistory
{
    public Guid Id { get; set; }

    /// <summary>
    /// Дата создания загруженного файла
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Откуда был загружен файл, например telegram или email
    /// </summary>
    public string Source { get; set; }

    /// <summary>
    /// Хэш файла для проверки на дубли
    /// </summary>
    public string FileHash { get; set; }
}