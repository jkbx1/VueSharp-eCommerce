using System.Security.Claims;
using Backend.Data;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly AppDbContext _context;

    public UserController(AppDbContext context)
    {
        _context = context;
    }

    private int GetCurrentUserId()
    {
        var idStr = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub")?.Value;
        if (int.TryParse(idStr, out var id)) return id;
        throw new UnauthorizedAccessException("Invalid token user.");
    }

    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var userId = GetCurrentUserId();
        var user = await _context.Users.FindAsync(userId);
        
        if (user == null) return NotFound();

        return Ok(new UserProfileDto
        {
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            DefaultAddress = user.DefaultAddress,
            DefaultCity = user.DefaultCity,
            DefaultZip = user.DefaultZip
        });
    }

    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UserProfileDto dto)
    {
        var userId = GetCurrentUserId();
        var user = await _context.Users.FindAsync(userId);
        
        if (user == null) return NotFound();

        user.FirstName = dto.FirstName;
        user.LastName = dto.LastName;
        user.DefaultAddress = dto.DefaultAddress;
        user.DefaultCity = dto.DefaultCity;
        user.DefaultZip = dto.DefaultZip;

        await _context.SaveChangesAsync();
        return Ok(new { message = "Profile updated successfully." });
    }

    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.CurrentPassword) || string.IsNullOrWhiteSpace(dto.NewPassword))
            return BadRequest(new { message = "Both passwords are required." });

        var userId = GetCurrentUserId();
        var user = await _context.Users.FindAsync(userId);
        
        if (user == null) return NotFound();

        if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash))
            return BadRequest(new { message = "Current password is incorrect." });

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
        await _context.SaveChangesAsync();
        
        return Ok(new { message = "Password updated successfully." });
    }

    [HttpDelete("account")]
    public async Task<IActionResult> DeleteAccount([FromBody] DeleteAccountDto dto)
    {
        var userId = GetCurrentUserId();
        var user = await _context.Users.FindAsync(userId);
        
        if (user == null) return NotFound();

        if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return BadRequest(new { message = "Invalid password. Cannot delete account." });

        // Unlink orders explicitly for an extra level of application safety over Entity Framework core default constraints 
        var userOrders = await _context.Orders.Where(o => o.UserId == userId).ToListAsync();
        foreach (var order in userOrders)
        {
            order.UserId = null;
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Account successfully deleted. All your previous orders remain intact anonymously." });
    }
}
