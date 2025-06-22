using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebStore.DTOs;
using WebStore.Services;
using webstore.filters;

namespace WebStore.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        AuthService authService,
        ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Регистрация нового пользователя
    /// </summary>
    
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponseDto), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        try
        {
            var response = await _authService.RegisterAsync(registerDto);
            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Registration failed for {Email}", registerDto.Email);
            return BadRequest(new ProblemDetails
            {
                Title = "Registration failed",
                Detail = ex.Message,
                Status = StatusCodes.Status400BadRequest
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration");
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Title = "Internal server error",
                Status = StatusCodes.Status500InternalServerError
            });
        }
    }

    /// <summary>
    /// Аутентификация пользователя
    /// </summary>
    [HttpPost("login")]
    [Authorize]
    [ProducesResponseType(typeof(LoginResponseDto), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 401)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        try
        {
            var response = await _authService.LoginAsync(loginDto);
            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Login failed for {Email}", loginDto.Email);
            return Unauthorized(new ProblemDetails
            {
                Title = "Authentication failed",
                Detail = ex.Message,
                Status = StatusCodes.Status401Unauthorized
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login");
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Title = "Internal server error",
                Status = StatusCodes.Status500InternalServerError
            });
        }
    }

    /// <summary>
    /// Проверка авторизации (тестовый метод)
    /// </summary>
    [HttpGet("test-auth")]
    [Authorize]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    public IActionResult TestAuth()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;

        return Ok(new
        {
            Message = "You are authorized!",
            UserId = userId,
            Email = userEmail
        });
    }

    /// <summary>
    /// Проверка роли Admin (тестовый метод)
    /// </summary>
    [HttpGet("test-admin")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(200)]
    [ProducesResponseType(403)]
    public IActionResult TestAdmin()
    {
        return Ok(new { Message = "You are admin!" });
    }
}