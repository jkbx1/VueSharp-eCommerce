namespace Backend.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal? SalePrice { get; set; }
    public string Category { get; set; } = string.Empty;
    public string SubCategory { get; set; } = string.Empty;
    public string ItemType { get; set; } = string.Empty;
    public List<string> AvailableSizes { get; set; } = new();
    public List<string> AvailableColors { get; set; } = new();
    public List<string> ImageUrls { get; set; } = new();
    public int StockQuantity { get; set; }
    public DateTime CreatedAt { get; set; } = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
}
