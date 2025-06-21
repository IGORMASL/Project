using WebStore.Data;
using WebStore.DTOs;
using WebStore.Models;
using WebStore.Repositories;
using WebStore.Services;

namespace WebStore.Services;

public class AuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly JwtService _jwtService; 

    public AuthService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        JwtService jwtService) 
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
    }

    public async Task<LoginResponseDto> RegisterAsync(RegisterDto registerDto)
    {
        if (await _userRepository.UserExistsAsync(registerDto.Email))
        {
            throw new ArgumentException("User with this email already exists");
        }

        var user = new User
        {
            Email = registerDto.Email,
            PasswordHash = _passwordHasher.HashPassword(registerDto.Password),
            FullName = registerDto.FullName,
            Role = UserRole.User,
            CreatedAt = DateTime.UtcNow
        };

        var createdUser = await _userRepository.CreateUserAsync(user);

        var token = _jwtService.GenerateToken(createdUser);

        return new LoginResponseDto
        {
            Token = token,
            Expires = DateTime.UtcNow.AddHours(1),
            User = new UserDto
            {
                Id = createdUser.Id,
                Email = createdUser.Email,
                FullName = createdUser.FullName,
                Role = createdUser.Role.ToString()
            }
        };
    }

    public async Task<LoginResponseDto> LoginAsync(LoginDto loginDto)
    {
        var user = await _userRepository.GetUserByEmailAsync(loginDto.Email);

        if (user == null || !_passwordHasher.VerifyPassword(user.PasswordHash, loginDto.Password))
        {
            throw new ArgumentException("Invalid email or password");
        }

        var token = _jwtService.GenerateToken(user);

        return new LoginResponseDto
        {
            Token = token,
            Expires = DateTime.UtcNow.AddHours(1),
            User = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role.ToString()
            }
        };
    }
}