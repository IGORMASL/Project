using System.ComponentModel.DataAnnotations;

namespace WebStore.DTOs;

public class UpdateUserDto
{
    [StringLength(100, ErrorMessage = "Full name must be less than 100 characters")]
    public string? FullName { get; set; }

    [EmailAddress(ErrorMessage = "Invalid email format")]
    [StringLength(100, ErrorMessage = "Email must be less than 100 characters")]
    public string? Email { get; set; }

    [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$",
        ErrorMessage = "Password must contain uppercase, lowercase and number")]
    public string? Password { get; set; }
}