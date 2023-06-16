using System.Net;
using Microsoft.Extensions.Logging;
using Polly;
using Services.Interfaces;
using Services.Models;

namespace Infrastructure.HttpClients;

public class ClientApiGatewayApiGateway : IClientApiGateway
{
    // Api клиента возвращает 204 код при отсутствии нового заказа, в этом случае повторяем запрос каждую секунду,
    // пока не будет получен ответ с заказом
    private static readonly IAsyncPolicy<HttpResponseMessage> NoContentPolicy = Policy<HttpResponseMessage>
        .HandleResult(message => message.StatusCode == HttpStatusCode.NoContent)
        .WaitAndRetryForeverAsync(_ => TimeSpan.FromSeconds(1));

    private readonly HttpClient _httpClient;
    private readonly ILogger<ClientApiGatewayApiGateway> _logger;

    public ClientApiGatewayApiGateway(HttpClient httpClient, ILogger<ClientApiGatewayApiGateway> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<OrdersFileInfo> GetNextOrderFile(DateTime lastTime)
    {
        try
        {
            // Предполагаемый набор query параметров. Взять один заказ поступивший после определенного времени.
            var uri = $"/Orders?from={lastTime:s}&take=1";
            
            // Политика ретраев и задержек при ошибках соединения, определена в httpClient
            var response = await NoContentPolicy.ExecuteAsync(async x 
                => await _httpClient.GetAsync(uri, x), CancellationToken.None);

            var fileStream = await response.Content.ReadAsStreamAsync();
            var fileName = response.Headers.GetValues("FileName").First();
            var createdAt = response.Headers.GetValues("CreatedAt").First();
            return new OrdersFileInfo
            {
                Content = fileStream,
                Name = fileName,
                CreatedAt = DateTime.Parse(createdAt)
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Ошибка при попытке обращения к API клиента");
            throw;
        }
    }
}