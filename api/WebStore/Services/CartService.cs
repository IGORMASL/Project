using WebStore.DTOs;
using WebStore.Models;
using WebStore.Repositories;

namespace WebStore.Services;

public class CartService
{
    private readonly ICartRepository _cartRepository;
    private readonly ICartItemRepository _cartItemRepository;
    private readonly IProductVariantRepository _productVariantRepository;

    public CartService(
        ICartRepository cartRepository,
        ICartItemRepository cartItemRepository,
        IProductVariantRepository productVariantRepository)
    {
        _cartRepository = cartRepository;
        _cartItemRepository = cartItemRepository;
        _productVariantRepository = productVariantRepository;
    }

    public async Task<CartDto> GetCartByUserIdAsync(Guid userId)
    {
        var cart = await _cartRepository.GetByUserIdAsync(userId);
        return MapToDto(cart);
    }

    public async Task<CartDto> AddItemToCartAsync(Guid userId, CartItemDto itemDto)
    {
        var cart = await _cartRepository.GetByUserIdAsync(userId);

        var variant = await _productVariantRepository.GetByIdAsync(itemDto.ProductVariantId);
        if (variant == null)
            throw new ArgumentException("Product variant not found");

        var existingItem = cart.CartItems.FirstOrDefault(ci => ci.ProductVariantId == itemDto.ProductVariantId);

        if (existingItem != null)
        {
            existingItem.Quantity += itemDto.Quantity;
            await _cartItemRepository.UpdateAsync(existingItem);
        }
        else
        {
            var newItem = new CartItem
            {
                CartId = cart.Id,
                ProductVariantId = itemDto.ProductVariantId,
                Quantity = itemDto.Quantity
            };
            await _cartItemRepository.CreateAsync(newItem);
        }

        return await GetCartByUserIdAsync(userId);
    }

    public async Task<CartDto> UpdateCartItemAsync(Guid userId, Guid itemId, UpdateCartItemDto dto)
    {
        var item = await _cartItemRepository.GetByIdAsync(itemId);
        if (item == null || item.Cart.UserId != userId)
            throw new ArgumentException("Item not found");

        item.Quantity = dto.Quantity;
        await _cartItemRepository.UpdateAsync(item);

        return await GetCartByUserIdAsync(userId);
    }

    public async Task<CartDto> RemoveItemFromCartAsync(Guid userId, Guid itemId)
    {
        var item = await _cartItemRepository.GetByIdAsync(itemId);
        if (item == null || item.Cart.UserId != userId)
            throw new ArgumentException("Item not found");

        await _cartItemRepository.DeleteAsync(itemId);
        return await GetCartByUserIdAsync(userId);
    }

    public async Task ClearCartAsync(Guid userId)
    {
        var cart = await _cartRepository.GetByUserIdAsync(userId);
        foreach (var item in cart.CartItems.ToList())
        {
            await _cartItemRepository.DeleteAsync(item.Id);
        }
    }

    private CartDto MapToDto(Cart cart)
    {
        return new CartDto
        {
            Id = cart.Id,
            UserId = cart.UserId,
            Items = cart.CartItems.Select(ci => new CartItemDto
            {
                Id = ci.Id,
                ProductVariantId = ci.ProductVariantId,
                Quantity = ci.Quantity,
                Price = ci.ProductVariant.Product.Price + ci.ProductVariant.AdditionalPrice
            }).ToList()
        };
    }
}