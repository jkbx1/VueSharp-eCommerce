using System;
using System.Security.Claims;
using Backend.Data;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly AppDbContext _context;

    public OrdersController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("my-orders")]
    public async Task<IActionResult> GetMyOrders()
    {
        var idStr = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub")?.Value;
        if (!int.TryParse(idStr, out var userId)) 
            return Unauthorized();

        var orders = await _context.Orders
            .Where(o => o.UserId == userId)
            .Include(o => o.Items)
                .ThenInclude(i => i.Product)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();

        return Ok(orders);
    }

    [HttpPost]
    [AllowAnonymous]
    [EnableRateLimiting("CheckoutLimiter")]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto dto)
    {
        if (dto.Items == null || !dto.Items.Any())
        {
            return BadRequest(new { message = "Order must contain at least one item." });
        }

        var order = new Order
        {
            Name = dto.Name,
            Email = dto.Email,
            Address = dto.Address,
            City = dto.City,
            Zip = dto.Zip,
            OrderDate = DateTime.UtcNow,
            TotalAmount = 0
        };

        // If user is authenticated, attach the record
        if (User.Identity?.IsAuthenticated == true)
        {
            var idStr = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub")?.Value;
            if (int.TryParse(idStr, out var userId))
            {
                order.UserId = userId;
            }
        }

        // ── M-1: Batch product fetch — single DB round-trip via WHERE IN ─────
        // Fetch all products referenced in this order in one query.
        var requestedProductIds = dto.Items.Select(i => i.ProductId).Distinct().ToList();
        var products = await _context.Products
            .Where(p => requestedProductIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id);

        // Validate all products exist before touching any state
        foreach (var itemDto in dto.Items)
        {
            if (!products.ContainsKey(itemDto.ProductId))
                return BadRequest(new { message = $"Product with ID {itemDto.ProductId} not found." });
        }

        foreach (var itemDto in dto.Items)
        {
            var product = products[itemDto.ProductId];

            if (product.StockQuantity < itemDto.Quantity)
            {
                return BadRequest(new { message = $"Insufficient stock for product '{product.Name}'. Available: {product.StockQuantity}, Requested: {itemDto.Quantity}." });
            }

            // Deduct stock
            product.StockQuantity -= itemDto.Quantity;

            // Calculate price based on SalePrice if available
            var unitPrice = product.SalePrice ?? product.Price;
            
            var orderItem = new OrderItem
            {
                ProductId = product.Id,
                SelectedSize = itemDto.SelectedSize,
                SelectedColor = itemDto.SelectedColor,
                Quantity = itemDto.Quantity,
                UnitPrice = unitPrice
            };

            order.TotalAmount += unitPrice * itemDto.Quantity;
            order.Items.Add(orderItem);
        }

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        return Created($"/api/orders/{order.Id}", new { orderId = order.Id, totalAmount = order.TotalAmount });
    }

    [HttpGet("track")]
    [AllowAnonymous]
    public async Task<IActionResult> TrackOrder([FromQuery] int orderId, [FromQuery] string email)
    {
        var order = await _context.Orders
            .Include(o => o.Items)
                .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order == null || !string.Equals(order.Email, email, StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest(new { message = "Order not found. Please check your details." });
        }

        return Ok(order);
    }

    [HttpPost("{id}/claim")]
    [Authorize]
    public async Task<IActionResult> ClaimOrder(int id)
    {
        var idStr = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub")?.Value;
        if (!int.TryParse(idStr, out var userId)) 
            return Unauthorized();
            
        var userEmail = User.FindFirstValue(ClaimTypes.Email) ?? User.FindFirst("email")?.Value;

        var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == id);
        
        if (order == null)
            return NotFound(new { message = "Order not found." });

        if (order.UserId != null)
            return BadRequest(new { message = "Order already claimed." });

        if (!string.Equals(order.Email, userEmail, StringComparison.OrdinalIgnoreCase))
            return StatusCode(403, new { message = "Forbidden: email mismatch." });

        order.UserId = userId;
        await _context.SaveChangesAsync();

        return Ok(new { message = "Order claimed successfully." });
    }

    [HttpDelete("{orderId}")]
    [Authorize]
    public async Task<IActionResult> CancelOrder(int orderId)
    {
        var idStr = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub")?.Value;
        if (!int.TryParse(idStr, out var userId)) 
            return Unauthorized();

        var order = await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);

        if (order == null)
            return NotFound(new { message = "Order not found or access denied." });

        // ── M-1: Batch stock restore — single DB round-trip via WHERE IN ──────
        // Old code: FindAsync(item.ProductId) inside a foreach = N database queries for N items.
        // New code: one SELECT WHERE Id IN (...) = 1 database query regardless of item count.
        var productIds = order.Items.Select(i => i.ProductId).Distinct().ToList();
        var products = await _context.Products
            .Where(p => productIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id);

        foreach (var item in order.Items)
        {
            if (products.TryGetValue(item.ProductId, out var product))
            {
                product.StockQuantity += item.Quantity;
            }
        }

        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Order cancelled and stock restored successfully." });
    }
}
