namespace Backend.Models;

public class OrderItem
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public string SelectedSize { get; set; } = string.Empty;
    public string SelectedColor { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }

    public Order? Order { get; set; }
    public Product? Product { get; set; }
}
