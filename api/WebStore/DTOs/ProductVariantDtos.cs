using WebStore.DTOs;

public class ProductVariantDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string Size { get; set; }
    public string Color { get; set; }
    public int StockQuantity { get; set; }
    public decimal AdditionalPrice { get; set; }
    public ProductDto? Product { get; set; } 
}