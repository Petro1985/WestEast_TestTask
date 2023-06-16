namespace Services.Models;

public class OrderModel
{
    public int Id { get; set; }
    public DateTime OrderTime { get; set; }
    public string ProductModel { get; set; }
    public int Quantity { get; set; }
}