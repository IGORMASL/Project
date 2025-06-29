using WebStore.Models;

namespace WebStore.Repositories;

public interface IReviewRepository
{
    Task<Review> CreateAsync(Review review);
    Task<Review?> GetByIdAsync(Guid id);
    Task<IEnumerable<Review>> GetByProductIdAsync(Guid productId);
    Task<IEnumerable<Review>> GetByUserIdAsync(Guid userId);
    Task<Review> UpdateAsync(Review review);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> HasUserReviewedProduct(Guid userId, Guid productId); 
}