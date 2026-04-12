using System.ComponentModel.DataAnnotations;

namespace Backend.Models;

public class CreateOrderDto
{
    [Required, StringLength(50)]
    public string Name { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(100)]
    public string Email { get; set; } = string.Empty;

    [Required, StringLength(150)]
    public string Address { get; set; } = string.Empty;

    [Required, StringLength(50)]
    public string City { get; set; } = string.Empty;

    [Required, StringLength(10)]
    public string Zip { get; set; } = string.Empty;

    [Required, MinLength(1)]
    public List<OrderItemDto> Items { get; set; } = new();
}

public class OrderItemDto 
{
    public int ProductId { get; set; }
    public string SelectedSize { get; set; } = string.Empty;
    public string SelectedColor { get; set; } = string.Empty;
    public int Quantity { get; set; }
}
