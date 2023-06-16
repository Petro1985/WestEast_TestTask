using Services.Interfaces;
using Services.Models;

namespace Infrastructure.Services;

public class OrderService : IOrderService
{
    public Task SendNewOrders(IEnumerable<OrderModel> orders)
    {
        throw new NotImplementedException();
    }
}