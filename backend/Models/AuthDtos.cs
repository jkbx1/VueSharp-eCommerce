using System.ComponentModel.DataAnnotations;

namespace Backend.Models;

/// <summary>
/// Shared password policy regex.
/// Requires: min 8 chars, 1 uppercase, 1 lowercase, 1 digit, 1 special char (@$!%*?&).
/// </summary>
internal static class PasswordPolicy
{
    public const string Regex = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$";
    public const string ErrorMessage = "Password must be at least 8 characters and contain at least one uppercase letter, one lowercase letter, one number, and one special character (@$!%*?&).";
}

public class RegisterDto
{
    [Required, EmailAddress, StringLength(100)]
    public string Email { get; set; } = string.Empty;

    [Required, RegularExpression(PasswordPolicy.Regex, ErrorMessage = PasswordPolicy.ErrorMessage), StringLength(50, MinimumLength = 8)]
    public string Password { get; set; } = string.Empty;
}

public class LoginDto
{
    [Required, EmailAddress, StringLength(100)]
    public string Email { get; set; } = string.Empty;

    [Required, StringLength(50)]
    public string Password { get; set; } = string.Empty;
}

