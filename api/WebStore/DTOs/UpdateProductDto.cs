using System.ComponentModel.DataAnnotations;

namespace WebStore.DTOs;

public class UpdateProductDto
{
    [StringLength(100)]
    public string? Name { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    [Range(0.01, 1000000)]
    public decimal? Price { get; set; }
}