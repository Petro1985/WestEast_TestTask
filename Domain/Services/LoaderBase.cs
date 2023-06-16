using System.Security.Cryptography;
using Microsoft.Extensions.Logging;
using Services.Interfaces;
using Services.Models;
using Services.Parsers;

namespace Services.Services;

public abstract class LoaderBase
{
    private readonly ILoadHistoryRepository _loadHistoryRepository;
    private readonly ILogger _logger;
    private readonly IEnumerable<IOrderParser> _parsers;

    protected LoaderBase(ILogger logger, IEnumerable<IOrderParser> parsers,
        ILoadHistoryRepository loadHistoryRepository)
    {
        _logger = logger;
        _parsers = parsers;
        _loadHistoryRepository = loadHistoryRepository;
    }

    public abstract string SourceName { get; }
    protected abstract Task<OrdersFileInfo> GetNextFile(DateTime lasTime);
    protected abstract ValidationResult Validate(MemoryStream file);
    protected abstract Task SendOrders(IEnumerable<OrderModel> orders);

    private async Task<IEnumerable<OrderModel>> Parser(MemoryStream file, string fileName)
    {
        var parser = _parsers.Single(x => x.CanParse(fileName));
        return await parser.Parse(file);
    }

    public async Task Load()
    {
        var lastTime = await _loadHistoryRepository.GetLastEntryTime(SourceName);
        var file = await GetNextFile(lastTime);
        var fileInMemory = await ReadFile(file.Content);
        var result = Validate(fileInMemory);

        if (!result.IsSuccess)
        {
            ValidationErrorFallBack(result);
            return;
        }
        
        var checkSum = await CalculateCheckSum(fileInMemory);

        if (await CheckIfItAlreadyLoaded(checkSum))
        {
            _logger.LogWarning("Файл {FileName} уже был загружен", file.Name);
            return;
        }

        fileInMemory.Position = 0;
        var orders = await Parser(fileInMemory, file.Name);

        await SaveLoadInfoToHistory(file.CreatedAt, checkSum);

        await SendOrders(orders);
    }

    private async Task<string> CalculateCheckSum(MemoryStream fileInMemory)
    {
        using var md5 = MD5.Create();
        fileInMemory.Position = 0;
        return Convert.ToBase64String(await md5.ComputeHashAsync(fileInMemory));
    }

    private async Task SaveLoadInfoToHistory(DateTime createdAt, string checkSum)
    {
        await _loadHistoryRepository.SaveLoadInfo(createdAt, SourceName, checkSum);
    }

    private async Task<bool> CheckIfItAlreadyLoaded(string checkSum)
    {
        return await _loadHistoryRepository.HashAlreadyExists(checkSum);
    }

    protected virtual void ValidationErrorFallBack(ValidationResult validationResult)
    {
        _logger.LogWarning("Validation error occurred. {Errors}",
            string.Join('\n', validationResult.Errors));
        // Можно предусмотреть разные варианты информирования пользователей о том, что их заказ не прошел валидацию
        // на данном этапе проста пишем в logWarning и игнорируем это файл
    }

    private async Task<MemoryStream> ReadFile(Stream file)
    {
        var copy = new MemoryStream();
        await file.CopyToAsync(copy);
        copy.Position = 0;
        return copy;
    }
}