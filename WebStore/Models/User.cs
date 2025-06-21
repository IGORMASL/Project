using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebStore.Models;

public class User
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string PasswordHash { get; set; }

    [Required]
    public string FullName { get; set; }

    [Required]
    public UserRole Role { get; set; } = UserRole.User;

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}