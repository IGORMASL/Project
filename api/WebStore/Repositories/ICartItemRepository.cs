using WebStore.Models;

namespace WebStore.Repositories;

public interface ICartItemRepository
{
    Task<CartItem?> GetByIdAsync(Guid id);
    Task<CartItem> CreateAsync(CartItem item);
    Task<CartItem> UpdateAsync(CartItem item);
    Task<bool> DeleteAsync(Guid id);
}