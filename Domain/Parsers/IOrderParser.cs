using Services.Models;

namespace Services.Parsers;

/// <summary>
///     Парсер файлов заказов
/// </summary>
public interface IOrderParser
{
    public bool CanParse(string fileName);
    public Task<IEnumerable<OrderModel>> Parse(Stream file);
}