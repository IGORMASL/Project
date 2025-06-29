using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using WebStore.DTOs;
using WebStore.Models;
using WebStore.Repositories;

namespace WebStore.Services;

public class UserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<UserService> _logger;

    public UserService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    public async Task<User> GetUserByEmailAsync(string email)
    {
        var user = await _userRepository.GetUserByEmailAsync(email);
        if (user == null)
        {
            _logger.LogWarning("User with email {Email} not found", email);
            throw new ArgumentException("User not found");
        }
        return user;
    }

    public async Task<UserDto> GetUserByIdAsync(Guid id)
    {
        var user = await _userRepository.GetUserByIdAsync(id);
        return user == null
            ? throw new ArgumentException("User not found")
            : MapToDto(user);
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        var users = await _userRepository.GetAllUsersAsync();
        return users.Select(MapToDto);
    }

    public async Task<UserDto> UpdateUserAsync(Guid id, UpdateUserDto dto)
    {
        var user = await _userRepository.GetUserByIdAsync(id)
            ?? throw new ArgumentException("User not found");

        if (!string.IsNullOrEmpty(dto.Email))
        {
            if (await _userRepository.UserExistsAsync(dto.Email) &&
                !string.Equals(user.Email, dto.Email, StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Email already in use");
            }
            user.Email = dto.Email.ToLower().Trim();
        }

        if (!string.IsNullOrEmpty(dto.FullName))
            user.FullName = dto.FullName.Trim();

        if (!string.IsNullOrEmpty(dto.Password))
        {
            if (dto.Password.Length < 8)
                throw new ArgumentException("Password must be at least 8 characters");
            user.PasswordHash = _passwordHasher.HashPassword(dto.Password);
        }

        user.UpdatedAt = DateTime.UtcNow;
        var updatedUser = await _userRepository.UpdateUserAsync(user);
        return MapToDto(updatedUser);
    }

    public async Task<bool> DeleteUserAsync(Guid id)
    {
        try
        {
            var result = await _userRepository.DeleteUserAsync(id);
            if (!result)
            {
                _logger.LogWarning("Failed to delete user with ID {UserId}", id);
            }
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user with ID {UserId}", id);
            return false;
        }
    }

    [Authorize(Roles = "Admin")]
    public async Task<UserDto> ChangeUserRoleAsync(Guid id, UserRole newRole)
    {
        var user = await _userRepository.GetUserByIdAsync(id)
            ?? throw new ArgumentException("User not found");

        if (user.Role == UserRole.Admin && newRole != UserRole.Admin)
            throw new UnauthorizedAccessException("Cannot change admin role");

        user.Role = newRole;
        user.UpdatedAt = DateTime.UtcNow;

        var updatedUser = await _userRepository.UpdateUserAsync(user);
        return MapToDto(updatedUser);
    }

    public async Task<bool> CheckEmailAvailableAsync(string email)
    {
        try
        {
            await GetUserByEmailAsync(email);
            return false;
        }
        catch (ArgumentException)
        {
            return true;
        }
    }

    private static UserDto MapToDto(User user) => new()
    {
        Id = user.Id,
        Email = user.Email,
        FullName = user.FullName,
        Role = user.Role.ToString(),
        CreatedAt = user.CreatedAt
    };
}