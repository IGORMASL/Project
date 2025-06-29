using WebStore.Models;

namespace WebStore.Repositories;

public interface IUserRepository
{
    Task<User> CreateUserAsync(User user);
    Task<User?> GetUserByEmailAsync(string email);
    Task<User?> GetUserByIdAsync(Guid id);
    Task<bool> UserExistsAsync(string email);
    Task<User> UpdateUserAsync(User user);
    Task<User> PatchUserAsync(Guid id, Action<User> patch);
    Task<bool> DeleteUserAsync(Guid id);
    Task<IEnumerable<User>> GetAllUsersAsync();
}