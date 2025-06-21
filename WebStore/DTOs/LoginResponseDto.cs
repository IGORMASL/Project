namespace WebStore.DTOs;

public class LoginResponseDto
{
    public string Token { get; set; }
    public DateTime Expires { get; set; }
    public UserDto User { get; set; }
}

public class UserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string FullName { get; set; }
    public string Role { get; set; }
}