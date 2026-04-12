using Backend.Data;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _db;

    public ProductsController(AppDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// GET /api/products/filters
    /// Returns unique filter options (SubCategory, ItemType, Sizes, Colors, Price Range) 
    /// for a given category/subcategory combination based on current DB stock.
    /// </summary>
    [HttpGet("filters")]
    public async Task<ActionResult<FilterDto>> GetFilters(
        [FromQuery] string[]? category,
        [FromQuery] string[]? subcategory,
        [FromQuery] string[]? itemType,
        [FromQuery] string[]? color,
        [FromQuery] string[]? size,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        [FromQuery] bool? onSale,
        [FromQuery] string? q)
    {
        // ── M-2: AsNoTracking — filters are read-only, no change tracking needed ─
        var baseQuery = _db.Products.AsNoTracking().AsQueryable();
        
        if (onSale.HasValue && onSale.Value)
            baseQuery = baseQuery.Where(p => p.SalePrice.HasValue && p.SalePrice < p.Price);

        if (!string.IsNullOrWhiteSpace(q))
        {
            var search = q.ToLower().Replace("-", "").Replace(" ", "");
            baseQuery = baseQuery.Where(p => 
                p.Name.ToLower().Replace("-", "").Replace(" ", "").Contains(search) || 
                p.Description.ToLower().Replace("-", "").Replace(" ", "").Contains(search) || 
                p.SKU.ToLower().Replace("-", "").Replace(" ", "").Contains(search));
        }

        // helper to apply specific group filters
        List<Product> ApplyFilters(string[]? cat = null, string[]? sub = null, string[]? type = null, string[]? col = null, string[]? sz = null)
        {
            var qry = baseQuery;
            if (cat != null && cat.Any()) qry = qry.Where(p => cat.Contains(p.Category));
            if (sub != null && sub.Any()) qry = qry.Where(p => sub.Contains(p.SubCategory));
            if (type != null && type.Any()) qry = qry.Where(p => type.Contains(p.ItemType));
            if (minPrice.HasValue) qry = qry.Where(p => (p.SalePrice ?? p.Price) >= minPrice.Value);
            if (maxPrice.HasValue) qry = qry.Where(p => (p.SalePrice ?? p.Price) <= maxPrice.Value);

            var list = qry.ToList();
            if (col != null && col.Any())
                list = list.Where(p => p.AvailableColors.Any(c => col.Contains(c))).ToList();
            if (sz != null && sz.Any())
                list = list.Where(p => p.AvailableSizes.Any(s => sz.Contains(s))).ToList();
            return list;
        }

        var productsForCats = ApplyFilters(null, subcategory, itemType, color, size);
        var productsForSubCats = ApplyFilters(category, null, itemType, color, size);
        var productsForStyles = ApplyFilters(category, subcategory, null, color, size);
        var productsForAttributes = ApplyFilters(category, subcategory, itemType, null, null);

        var filters = new FilterDto
        {
            Categories = productsForCats.Select(p => p.Category).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(c => c).ToList(),
            SubCategories = productsForSubCats.Select(p => p.SubCategory).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(s => s).ToList(),
            ItemTypes = productsForStyles.Select(p => p.ItemType).Where(t => !string.IsNullOrEmpty(t)).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(t => t).ToList(),
            Sizes = productsForAttributes.SelectMany(p => p.AvailableSizes).Distinct(StringComparer.OrdinalIgnoreCase).ToList(),
            Colors = productsForAttributes.SelectMany(p => p.AvailableColors).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(c => c).ToList(),
            MinPrice = productsForAttributes.Any() ? productsForAttributes.Min(p => p.SalePrice ?? p.Price) : 0,
            MaxPrice = productsForAttributes.Any() ? productsForAttributes.Max(p => p.SalePrice ?? p.Price) : 0,
            CategoryMapping = productsForCats
                .GroupBy(p => p.Category)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(p => p.SubCategory).Distinct(StringComparer.OrdinalIgnoreCase).ToList(),
                    StringComparer.OrdinalIgnoreCase
                )
        };

        return Ok(filters);
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedResult<ProductDto>>> GetProducts(
        [FromQuery] string[]? category,
        [FromQuery] string[]? subcategory,
        [FromQuery] string[]? itemType,
        [FromQuery] string[]? color,
        [FromQuery] string[]? size,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        [FromQuery] bool? onSale,
        [FromQuery] string? q,
        [FromQuery] string[]? skus,
        [FromQuery] string? sort,
        [FromQuery] int? limit,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 12)
    {
        // ── M-2: AsNoTracking — GetProducts is pure read, never writes ───────
        var query = _db.Products.AsNoTracking().AsQueryable();

        if (skus != null && skus.Any())
            query = query.Where(p => skus.Contains(p.SKU));

        if (!string.IsNullOrWhiteSpace(q))
        {
            var search = q.ToLower().Replace("-", "").Replace(" ", "");
            query = query.Where(p => 
                p.Name.ToLower().Replace("-", "").Replace(" ", "").Contains(search) || 
                p.Description.ToLower().Replace("-", "").Replace(" ", "").Contains(search) || 
                p.SKU.ToLower().Replace("-", "").Replace(" ", "").Contains(search));
        }

        if (category != null && category.Any())
            query = query.Where(p => category.Contains(p.Category));

        if (onSale.HasValue && onSale.Value)
        {
            query = query.Where(p => p.SalePrice.HasValue && p.SalePrice < p.Price);
            if (subcategory != null && subcategory.Any())
                query = query.Where(p => subcategory.Contains(p.SubCategory));
        }
        else if (subcategory != null && subcategory.Any())
        {
            query = query.Where(p => subcategory.Contains(p.SubCategory));
        }

        if (itemType != null && itemType.Any())
            query = query.Where(p => itemType.Contains(p.ItemType));

        if (minPrice.HasValue)
            query = query.Where(p => (p.SalePrice ?? p.Price) >= minPrice.Value);

        if (maxPrice.HasValue)
            query = query.Where(p => (p.SalePrice ?? p.Price) <= maxPrice.Value);

        // ── M-3: Pagination Strategy ──────────────────────────────────────────
        // Color/Size are stored as JSON arrays in SQLite and cannot be filtered
        // at the SQL level without a custom function or a schema change (join table).
        // When they are active, we must materialize and filter in-memory first.
        // When they are NOT active, we push sorting and pagination down to SQL —
        // executing COUNT(*) and SELECT ... LIMIT/OFFSET entirely in the database.

        bool needsInMemoryFilter = (color != null && color.Any()) || (size != null && size.Any());

        int totalCount;
        List<ProductDto> products;
        var actualPageSize = limit ?? pageSize;

        if (needsInMemoryFilter)
        {
            // In-memory path: materialize the SQL-filtered set, then apply color/size.
            // NOTE: We only project the fields we need to minimise allocation cost.
            var filteredList = await query.ToListAsync();

            if (color != null && color.Any())
                filteredList = filteredList.Where(p => p.AvailableColors.Any(c => color.Contains(c))).ToList();

            if (size != null && size.Any())
                filteredList = filteredList.Where(p => p.AvailableSizes.Any(s => size.Contains(s))).ToList();

            // Apply sorting in-memory
            IEnumerable<Product> sorted = sort switch
            {
                "newest"     => filteredList.OrderByDescending(p => p.CreatedAt),
                "price_asc"  => filteredList.OrderBy(p => p.SalePrice ?? p.Price),
                "price_desc" => filteredList.OrderByDescending(p => p.SalePrice ?? p.Price),
                _            => filteredList.OrderBy(p => p.Id)
            };

            totalCount = filteredList.Count;
            products = sorted
                .Skip((page - 1) * actualPageSize)
                .Take(actualPageSize)
                .Select(p => ProductDto.FromProduct(p))
                .ToList();
        }
        else
        {
            // ── M-3: SQL-level path — sort, count, and paginate in the database ──
            // This is the fast path for category/search/price browsing.
            IQueryable<Product> sorted = sort switch
            {
                "newest"     => query.OrderByDescending(p => p.CreatedAt),
                "price_asc"  => query.OrderBy(p => p.SalePrice ?? p.Price),
                "price_desc" => query.OrderByDescending(p => p.SalePrice ?? p.Price),
                _            => query.OrderBy(p => p.Id)
            };

            // COUNT(*) at SQL level — no full materialization needed.
            totalCount = await sorted.CountAsync();

            // SELECT with LIMIT/OFFSET at SQL level — only the page rows are loaded.
            products = await sorted
                .Skip((page - 1) * actualPageSize)
                .Take(actualPageSize)
                .Select(p => ProductDto.FromProduct(p))
                .ToListAsync();
        }

        return Ok(new PaginatedResult<ProductDto>
        {
            Items = products,
            TotalCount = totalCount,
            Page = page,
            PageSize = actualPageSize
        });
    }

    /// <summary>
    /// GET /api/products/{id}
    /// Returns a single product with computed sale fields.
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductDto>> GetProduct(int id)
    {
        // ── M-2: AsNoTracking — single product read, no update path ──────────
        var product = await _db.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        if (product is null) return NotFound();
        return Ok(ProductDto.FromProduct(product));
    }
}




