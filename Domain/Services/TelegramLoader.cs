using Microsoft.Extensions.Logging;
using Services.Interfaces;
using Services.Models;
using Services.Parsers;

namespace Services.Services;

public class TelegramLoader : LoaderBase
{
    public TelegramLoader(ILogger logger, IEnumerable<IOrderParser> parsers,
        ILoadHistoryRepository loadHistoryRepository) : base(logger, parsers, loadHistoryRepository)
    {
    }

    public override string SourceName => "Telegram";

    protected override Task<OrdersFileInfo> GetNextFile(DateTime lasTime)
    {
        throw new NotImplementedException();
    }

    protected override ValidationResult Validate(MemoryStream file)
    {
        throw new NotImplementedException();
    }

    protected override Task SendOrders(IEnumerable<OrderModel> orders)
    {
        throw new NotImplementedException();
    }
}