using System.ComponentModel.DataAnnotations;

namespace WebStore.DTOs;

public class CreateProductDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = null!;

    [StringLength(500)]
    public string? Description { get; set; }

    [Required]
    [Range(0.01, 1000000)]
    public decimal Price { get; set; }
}
