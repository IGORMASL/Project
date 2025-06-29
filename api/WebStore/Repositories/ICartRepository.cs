using WebStore.Models;

namespace WebStore.Repositories;

public interface ICartRepository
{
    Task<Cart> GetByUserIdAsync(Guid userId);
    Task<Cart> CreateAsync(Cart cart);
    Task<Cart> UpdateAsync(Cart cart);
}