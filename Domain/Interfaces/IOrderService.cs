using Services.Models;

namespace Services.Interfaces;

public interface IOrderService
{
    /// <summary>
    ///     Отправляет заказы в другой микросервис через брокер сообщений
    /// </summary>
    /// <param name="orders"></param>
    /// <returns></returns>
    public Task SendNewOrders(IEnumerable<OrderModel> orders);
}