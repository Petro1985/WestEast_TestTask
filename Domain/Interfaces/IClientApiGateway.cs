using Services.Models;

namespace Services.Interfaces;

public interface IClientApiGateway
{
    Task<OrdersFileInfo> GetNextOrderFile(DateTime lastTime);
}