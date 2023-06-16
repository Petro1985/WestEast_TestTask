using Services.Models;

namespace Services.Parsers;

public class OrderParserXlsx : IOrderParser
{
    public bool CanParse(string fileName)
    {
        return Path.GetExtension(fileName) == ".xlsx";
    }

    public Task<IEnumerable<OrderModel>> Parse(Stream file)
    {
        throw new NotImplementedException();
    }
}