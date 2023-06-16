using System.Text.Json;
using Services.Models;

namespace Services.Parsers;

public class OrderParserJson : IOrderParser
{
    public bool CanParse(string fileName)
    {
        return Path.GetExtension(fileName) == ".json";
    }

    public async Task<IEnumerable<OrderModel>> Parse(Stream file)
    {
        return await JsonSerializer.DeserializeAsync<IEnumerable<OrderModel>>(file)
               ?? Array.Empty<OrderModel>();
    }
}