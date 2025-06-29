using WebStore.Models;

namespace WebStore.Repositories;

public interface IOrderItemRepository
{
    Task<IEnumerable<OrderItem>> GetByOrderIdAsync(Guid orderId);
}