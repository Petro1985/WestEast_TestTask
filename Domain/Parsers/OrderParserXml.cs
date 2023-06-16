using Services.Models;

namespace Services.Parsers;

public class OrderParserXml : IOrderParser
{
    public bool CanParse(string fileName)
    {
        return Path.GetExtension(fileName) == ".xml";
    }

    public Task<IEnumerable<OrderModel>> Parse(Stream file)
    {
        throw new NotImplementedException();
    }
}