using System.Security.Cryptography;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using NSubstitute;
using Services.Interfaces;
using Services.Models;
using Services.Parsers;
using Services.Services;
using Xunit;

namespace Tests;

public class JsonLoaderTests
{
    [Fact]
    public async Task WebApiLoader_Should_Load_Json_File()
    {
        var parser = new OrderParserJson();
        var mockedLogger = Substitute.For<ILogger<WebApiLoader>>();
        var mockedRepo = Substitute.For<ILoadHistoryRepository>();
        var mockedOrderService = Substitute.For<IOrderService>();
        var mockerHttpClient = Substitute.For<IClientApiGateway>();

        var testedLoader = new WebApiLoader(new IOrderParser[] {parser}, mockedLogger, mockedRepo, mockedOrderService,
            mockerHttpClient);

        mockedRepo.GetLastEntryTime(Arg.Any<string>()).Returns(DateTime.MinValue);
        
        var testFile = new MemoryStream();
        var writer = new StreamWriter(testFile);
        var jsonText = """
            [
                {
                    "Id": 1,
                    "OrderTime": "2023-01-01",
                    "ProductModel": "Model",
                    "Quantity": 4
                },
                {
                    "Id": 2,
                    "OrderTime": "2023-01-02",
                    "ProductModel": "Model2",
                    "Quantity": 5
                }
            ]
        """;
        var expectedResult = new List<OrderModel>()
        {
            new ()
            {
                Id = 1,
                OrderTime = new DateTime(2023, 1, 1),
                ProductModel = "Model",
                Quantity = 4,
            },
            new ()
            {
                Id = 2,
                OrderTime = new DateTime(2023, 1, 2),
                ProductModel = "Model2",
                Quantity = 5,
            },
        };
        await writer.WriteAsync(jsonText);
        await writer.FlushAsync();
        
        var checkSum = "uMr2c9JhiGxmZr7yCIqOGw==";
        testFile.Position = 0;

        mockerHttpClient.GetNextOrderFile(Arg.Any<DateTime>()).Returns(new OrdersFileInfo()
            {Name = "test.json", CreatedAt = default, Content = testFile});

        await testedLoader.Load();

        // проверка правильности вызова метода соранения информации о загрузке
        await mockedRepo.Received().SaveLoadInfo(default, testedLoader.SourceName, checkSum);
        
        await mockedOrderService.Received().SendNewOrders(Arg.Is<IEnumerable<OrderModel>>(orders => MatchOrders(orders, expectedResult)));
    }
    
    static bool MatchOrders(IEnumerable<OrderModel> orders, IEnumerable<OrderModel> expectedResult)
    {
        orders.Should().BeEquivalentTo(expectedResult);
        return true;
    }

    [Fact]
    public async Task WebApiLoader_Should_Ignore_Duplicated_File()
    {
        var parser = new OrderParserJson();
        var mockedLogger = Substitute.For<ILogger<WebApiLoader>>();
        var mockedRepo = Substitute.For<ILoadHistoryRepository>();
        var mockedOrderService = Substitute.For<IOrderService>();
        var mockerHttpClient = Substitute.For<IClientApiGateway>();

        var testedLoader = new WebApiLoader(new IOrderParser[] {parser}, mockedLogger, mockedRepo, mockedOrderService,
            mockerHttpClient);

        var testFile = new MemoryStream();
        var writer = new StreamWriter(testFile);
        var jsonText = """
            [
                {
                    "Id": 1,
                    "OrderTime": "2023-01-01",
                    "ProductModel": "Model",
                    "Quantity": 4
                },
                {
                    "Id": 2,
                    "OrderTime": "2023-01-02",
                    "ProductModel": "Model2",
                    "Quantity": 5
                }
            ]
        """;
        await writer.WriteAsync(jsonText);
        await writer.FlushAsync();
        
        var checkSum = "uMr2c9JhiGxmZr7yCIqOGw==";
        testFile.Position = 0;

        mockedRepo.GetLastEntryTime(Arg.Any<string>()).Returns(DateTime.MinValue);
        mockedRepo.HashAlreadyExists(checkSum).Returns(true);
        
        mockerHttpClient.GetNextOrderFile(Arg.Any<DateTime>()).Returns(new OrdersFileInfo()
            {Name = "test.json", CreatedAt = default, Content = testFile});

        await testedLoader.Load();

        await mockedRepo.DidNotReceive().SaveLoadInfo(Arg.Any<DateTime>(), Arg.Any<string>(), Arg.Any<string>());
        await mockedOrderService.DidNotReceive().SendNewOrders(Arg.Any<IEnumerable<OrderModel>>());
    }
    
    
}