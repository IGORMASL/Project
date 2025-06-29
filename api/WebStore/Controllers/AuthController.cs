using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebStore.DTOs;
using Microsoft.Extensions.Logging;
using WebStore.Services;

namespace WebStore.Controllers
{
    [ApiController]
    [Route("api/auth")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly UserService _userService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            AuthService authService,
            UserService userService,
            ILogger<AuthController> logger)
        {
            _authService = authService;
            _userService = userService;
            _logger = logger;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            try
            {
                var response = await _authService.RegisterAsync(registerDto);
                _logger.LogInformation("New user registered: {Email}", registerDto.Email);
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Registration failed for {Email}", registerDto.Email);
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        [Authorize] // Теперь метод защищен авторизацией, как вы просили
        [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            try
            {
                var response = await _authService.LoginAsync(loginDto);
                _logger.LogInformation("User logged in: {Email}", loginDto.Email);
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Login failed for {Email}", loginDto.Email);
                return Unauthorized(new { message = ex.Message });
            }
        }

        [HttpGet("check-email")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<IActionResult> CheckEmailAvailable([FromQuery] string email)
        {
            var isAvailable = await _userService.CheckEmailAvailableAsync(email);
            return Ok(isAvailable);
        }

        [HttpGet("me")]
        [Authorize]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userService.GetUserByIdAsync(Guid.Parse(userId));
            return Ok(user);
        }
    }
}