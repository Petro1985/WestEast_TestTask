namespace Services.Models;

public class OrdersFileInfo
{
    public Stream Content { get; set; }
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; }
}