using WebStore.Models;

namespace WebStore.Repositories;

public interface IProductRepository
{
    Task<Product> CreateAsync(Product product);
    Task<Product?> GetByIdAsync(Guid id);
    Task<IEnumerable<Product>> GetAllAsync();
    Task<Product> UpdateAsync(Product product);
    Task<ProductVariant?> GetVariantByIdAsync(Guid variantId);
    Task<bool> DeleteAsync(Guid id);
}