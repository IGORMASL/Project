using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebStore.DTOs;
using WebStore.Models;
using WebStore.Services;

namespace WebStore.Controllers;

[ApiController]
[Route("api/users")]
[Produces("application/json")]
public class UsersController : ControllerBase
{
    private readonly UserService _userService;
    private readonly OrderService _orderService;
    private readonly ReviewService _reviewService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(
        UserService userService,
        OrderService orderService,
        ReviewService reviewService,
        ILogger<UsersController> logger)
    {
        _userService = userService;
        _orderService = orderService;
        _reviewService = reviewService;
        _logger = logger;
    }

    /// <summary>
    /// Получить список всех пользователей (только для администраторов)
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(IEnumerable<UserDto>), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }

    /// <summary>
    /// Получить информацию о пользователе по ID
    /// </summary>
    [HttpGet("{id}")]
    [Authorize]
    [ProducesResponseType(typeof(UserWithDetailsDto), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        try
        {
            if (!User.IsInRole("Admin") && GetCurrentUserId() != id)
                return Forbid();

            var user = await _userService.GetUserByIdAsync(id);
            var orders = await _orderService.GetUserOrdersAsync(id);
            var reviews = await _reviewService.GetUserReviewsAsync(id);

            var result = new UserWithDetailsDto
            {
                User = user,
                Orders = orders,
                Reviews = reviews
            };

            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "User not found with ID: {UserId}", id);
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Получить заказы пользователя
    /// </summary>
    [HttpGet("{id}/orders")]
    [Authorize]
    [ProducesResponseType(typeof(IEnumerable<OrderDto>), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<IActionResult> GetUserOrders(Guid id)
    {
        if (!User.IsInRole("Admin") && GetCurrentUserId() != id)
            return Forbid();

        var orders = await _orderService.GetUserOrdersAsync(id);
        return Ok(orders);
    }

    /// <summary>
    /// Получить отзывы пользователя
    /// </summary>
    [HttpGet("{id}/reviews")]
    [Authorize]
    [ProducesResponseType(typeof(IEnumerable<ReviewDto>), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<IActionResult> GetUserReviews(Guid id)
    {
        if (!User.IsInRole("Admin") && GetCurrentUserId() != id)
            return Forbid();

        var reviews = await _reviewService.GetUserReviewsAsync(id);
        return Ok(reviews);
    }

    /// <summary>
    /// Обновить информацию о пользователе
    /// </summary>
    [HttpPut("{id}")]
    [Authorize]
    [ProducesResponseType(typeof(UserDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserDto dto)
    {
        try
        {
            if (!User.IsInRole("Admin") && GetCurrentUserId() != id)
                return Forbid();

            var updatedUser = await _userService.UpdateUserAsync(id, dto);
            return Ok(updatedUser);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Failed to update user with ID: {UserId}", id);
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Удалить пользователя (только для администраторов)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(204)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        try
        {
            var result = await _userService.DeleteUserAsync(id);
            return result ? NoContent() : NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user with ID: {UserId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Изменить роль пользователя (только для администраторов)
    /// </summary>
    [HttpPatch("{id}/role")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(UserDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<IActionResult> ChangeUserRole(Guid id, [FromBody] ChangeRoleRequest request)
    {
        try
        {
            var updatedUser = await _userService.ChangeUserRoleAsync(id, request.NewRole);
            return Ok(updatedUser);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    private Guid GetCurrentUserId()
    {
        return Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
    }
}

public class UserWithDetailsDto
{
    public UserDto User { get; set; }
    public IEnumerable<OrderDto> Orders { get; set; }
    public IEnumerable<ReviewDto> Reviews { get; set; }
}

public class ChangeRoleRequest
{
    public UserRole NewRole { get; set; }
}