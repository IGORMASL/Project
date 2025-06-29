using WebStore.Models;

namespace WebStore.Repositories;

public interface ICategoryRepository
{
    Task<Category> CreateAsync(Category category);
    Task<Category?> GetByIdAsync(Guid id);
    Task<IEnumerable<Category>> GetAllAsync();
    Task<Category> UpdateAsync(Category category);
    Task<bool> DeleteAsync(Guid id);
}