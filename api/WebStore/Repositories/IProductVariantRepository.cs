// IProductVariantRepository.cs
using WebStore.Models;

public interface IProductVariantRepository
{
    Task<ProductVariant?> GetByIdAsync(Guid id);
    Task<ProductVariant> CreateAsync(ProductVariant variant);
    Task<ProductVariant> UpdateAsync(ProductVariant variant);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> IsInStockAsync(Guid variantId, int quantity);
    Task<IEnumerable<ProductVariant>> GetAllByProductIdAsync(Guid productId); 
}