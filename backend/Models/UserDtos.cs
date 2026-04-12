using System.ComponentModel.DataAnnotations;

namespace Backend.Models;

public class UserProfileDto
{
    [Required, EmailAddress, StringLength(100)]
    public string Email { get; set; } = string.Empty;

    [Required, StringLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Required, StringLength(50)]
    public string LastName { get; set; } = string.Empty;

    [Required, StringLength(150)]
    public string DefaultAddress { get; set; } = string.Empty;

    [Required, StringLength(50)]
    public string DefaultCity { get; set; } = string.Empty;

    [Required, StringLength(10)]
    public string DefaultZip { get; set; } = string.Empty;
}

public class ChangePasswordDto
{
    [Required, StringLength(50)]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required, RegularExpression(PasswordPolicy.Regex, ErrorMessage = PasswordPolicy.ErrorMessage), StringLength(50, MinimumLength = 8)]
    public string NewPassword { get; set; } = string.Empty;
}

public class DeleteAccountDto
{
    [Required, StringLength(50)]
    public string Password { get; set; } = string.Empty;
}

