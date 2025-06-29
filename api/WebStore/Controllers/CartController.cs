using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebStore.DTOs;
using WebStore.Services;

namespace WebStore.Controllers;

[ApiController]
[Route("api/cart")]
[Authorize]
public class CartController : ControllerBase
{
    private readonly CartService _cartService;
    private readonly ProductService _productService;

    public CartController(
        CartService cartService,
        ProductService productService)
    {
        _cartService = cartService;
        _productService = productService;
    }

    /// <summary>
    /// Получить содержимое корзины
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(CartDto), 200)]
    public async Task<IActionResult> GetCart()
    {
        var userId = GetCurrentUserId();
        var cart = await _cartService.GetCartByUserIdAsync(userId);
        return Ok(cart);
    }

    /// <summary>
    /// Добавить товар в корзину
    /// </summary>
    [HttpPost("items")]
    [ProducesResponseType(typeof(CartDto), 200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> AddItem([FromBody] AddToCartDto dto)
    {
        try
        {
            // Проверка доступности товара
            var variant = await _productService.GetVariantByIdAsync(dto.ProductVariantId);
            if (variant.StockQuantity < dto.Quantity)
            {
                return BadRequest("Not enough stock available");
            }

            var userId = GetCurrentUserId();
            var cart = await _cartService.AddItemToCartAsync(userId, new CartItemDto
            {
                ProductVariantId = dto.ProductVariantId,
                Quantity = dto.Quantity,
                Price = variant.Product.Price + variant.AdditionalPrice
            });

            return Ok(cart);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Изменить количество товара в корзине
    /// </summary>
    [HttpPut("items/{itemId}")]
    [ProducesResponseType(typeof(CartDto), 200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> UpdateItem(Guid itemId, [FromBody] UpdateCartItemDto dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            var cart = await _cartService.UpdateCartItemAsync(userId, itemId, dto);
            return Ok(cart);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Удалить товар из корзины
    /// </summary>
    [HttpDelete("items/{itemId}")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> RemoveItem(Guid itemId)
    {
        var userId = GetCurrentUserId();
        await _cartService.RemoveItemFromCartAsync(userId, itemId);
        return NoContent();
    }

    private Guid GetCurrentUserId()
    {
        return Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
    }
}

public class AddToCartDto
{
    public Guid ProductVariantId { get; set; }
    public int Quantity { get; set; }
}