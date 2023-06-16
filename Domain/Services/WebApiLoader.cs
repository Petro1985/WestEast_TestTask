using Microsoft.Extensions.Logging;
using Services.Interfaces;
using Services.Models;
using Services.Parsers;

namespace Services.Services;

public class WebApiLoader : LoaderBase
{
    private readonly IClientApiGateway _customerHttpClient;
    private readonly IOrderService _orderService;

    public WebApiLoader(IEnumerable<IOrderParser> parsers,
        ILogger<WebApiLoader> logger, ILoadHistoryRepository loadHistoryRepo, IOrderService orderService,
        IClientApiGateway customerHttpClient)
        : base(logger, parsers, loadHistoryRepo)
    {
        _orderService = orderService;
        _customerHttpClient = customerHttpClient;
    }

    public override string SourceName => "WebApi";

    protected override async Task<OrdersFileInfo> GetNextFile(DateTime lastTime)
    {
        return await _customerHttpClient.GetNextOrderFile(lastTime);
    }

    protected override ValidationResult Validate(MemoryStream file)
    {
        // при реализации вынести в конфиг
        const int maxFileLength = 100500;
        if (file.Length > maxFileLength) return ValidationResult.Fail("Слишком большой файл");

        // Тут нужно дописать логику валидации
        return ValidationResult.Success();
    }

    protected override async Task SendOrders(IEnumerable<OrderModel> orders)
    {
        await _orderService.SendNewOrders(orders);
    }
}