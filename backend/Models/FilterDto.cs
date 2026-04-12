namespace Backend.Models;

public class FilterDto
{
    public List<string> Categories { get; set; } = new();
    public List<string> SubCategories { get; set; } = new();
    public List<string> ItemTypes { get; set; } = new();
    public List<string> Sizes { get; set; } = new();
    public List<string> Colors { get; set; } = new();
    public decimal MinPrice { get; set; }
    public decimal MaxPrice { get; set; }

    // Mapping of which Subcategories belong to which Category in the current result set
    public Dictionary<string, List<string>> CategoryMapping { get; set; } = new();
}
