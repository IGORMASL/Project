using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebStore.Models;

public class ProductVariant
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid ProductId { get; set; }

    [ForeignKey("ProductId")]
    public Product Product { get; set; }

    [Required]
    [StringLength(50)]
    public string Size { get; set; }

    [Required]
    [StringLength(50)]
    public string Color { get; set; }

    [Required]
    public int StockQuantity { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal AdditionalPrice { get; set; }

    public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}