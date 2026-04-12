namespace Backend.Models;

/// <summary>
/// Response DTO returned by the API. Contains all product fields plus
/// computed sale properties so the frontend never needs business logic.
/// </summary>
public class ProductDto
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
    public DateTime CreatedAt { get; set; }

    // --- Computed by backend ---
    public bool IsOnSale { get; set; }
    public int? DiscountPercent { get; set; }
    public decimal DisplayPrice { get; set; }

    public static ProductDto FromProduct(Product p)
    {
        bool onSale = p.SalePrice.HasValue && p.SalePrice.Value < p.Price;
        int? discount = onSale
            ? (int)Math.Round((p.Price - p.SalePrice!.Value) / p.Price * 100)
            : null;

        return new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            SKU = p.SKU,
            Price = p.Price,
            SalePrice = p.SalePrice,
            Category = p.Category,
            SubCategory = p.SubCategory,
            ItemType = p.ItemType,
            AvailableSizes = p.AvailableSizes,
            AvailableColors = p.AvailableColors,
            ImageUrls = p.ImageUrls,
            StockQuantity = p.StockQuantity,
            CreatedAt = p.CreatedAt,
            IsOnSale = onSale,
            DiscountPercent = discount,
            DisplayPrice = p.SalePrice ?? p.Price,
        };
    }
}
