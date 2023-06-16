namespace DAL.Entity;

public class Order
{
    public Guid Id { get; set; }
    public string ProductModel { get; set; }
    public int Quantity { get; set; }
    public DateTime CreatedAt { get; set; }
}