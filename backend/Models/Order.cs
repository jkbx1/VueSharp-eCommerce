namespace Backend.Models;

public class Order
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Zip { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public DateTime OrderDate { get; set; }

    public int? UserId { get; set; }
    public User? User { get; set; }

    public List<OrderItem> Items { get; set; } = new();
}
