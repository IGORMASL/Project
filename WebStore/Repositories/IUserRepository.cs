using WebStore.Models;

namespace WebStore.Repositories;

public interface IUserRepository
{
    Task<User> CreateUserAsync(User user);
    Task<User?> GetUserByEmailAsync(string email);
    Task<bool> UserExistsAsync(string email);
}