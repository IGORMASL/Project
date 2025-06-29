using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebStore.Models;

public class CartItem
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid CartId { get; set; }

    [ForeignKey("CartId")]
    public Cart Cart { get; set; }

    [Required]
    public Guid ProductVariantId { get; set; }

    [ForeignKey("ProductVariantId")]
    public ProductVariant ProductVariant { get; set; }

    [Required]
    public int Quantity { get; set; }
}