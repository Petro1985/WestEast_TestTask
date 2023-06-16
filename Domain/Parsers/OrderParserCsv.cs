using Services.Models;

namespace Services.Parsers;

public class OrderParserCsv : IOrderParser
{
    public bool CanParse(string fileName)
    {
        return Path.GetExtension(fileName) == ".csv";
    }

    public Task<IEnumerable<OrderModel>> Parse(Stream file)
    {
        throw new NotImplementedException();
    }
}