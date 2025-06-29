using Microsoft.EntityFrameworkCore;
using WebStore.DTOs;
using WebStore.Models;
using WebStore.Repositories;

namespace WebStore.Services;

public class ProductVariantService
{
    private readonly IProductVariantRepository _variantRepository;
    private readonly IProductRepository _productRepository;

    public ProductVariantService(
        IProductVariantRepository variantRepository,
        IProductRepository productRepository)
    {
        _variantRepository = variantRepository;
        _productRepository = productRepository;
    }

    public async Task<ProductVariantDto> CreateVariantAsync(Guid productId, ProductVariantDto dto)
    {
        var product = await _productRepository.GetByIdAsync(productId);
        if (product == null)
            throw new ArgumentException("Product not found");

        var variant = new ProductVariant
        {
            ProductId = productId,
            Size = dto.Size,
            Color = dto.Color,
            StockQuantity = dto.StockQuantity,
            AdditionalPrice = dto.AdditionalPrice
        };

        var created = await _variantRepository.CreateAsync(variant);
        return MapToDto(created);
    }

    public async Task<ProductVariantDto> GetByIdAsync(Guid id)
    {
        var variant = await _variantRepository.GetByIdAsync(id);
        if (variant == null)
            throw new ArgumentException("Variant not found");

        return MapToDto(variant);
    }

    public async Task<ProductVariantDto> UpdateVariantAsync(Guid id, ProductVariantDto dto)
    {
        var variant = await _variantRepository.GetByIdAsync(id);
        if (variant == null)
            throw new ArgumentException("Variant not found");

        variant.Size = dto.Size;
        variant.Color = dto.Color;
        variant.StockQuantity = dto.StockQuantity;
        variant.AdditionalPrice = dto.AdditionalPrice;

        var updated = await _variantRepository.UpdateAsync(variant);
        return MapToDto(updated);
    }

    public async Task<bool> DeleteVariantAsync(Guid id)
    {
        return await _variantRepository.DeleteAsync(id);
    }

    public async Task<bool> IsInStockAsync(Guid variantId, int quantity)
    {
        return await _variantRepository.IsInStockAsync(variantId, quantity);
    }

    public async Task UpdateStockAsync(Guid variantId, int quantityChange)
    {
        var variant = await _variantRepository.GetByIdAsync(variantId);
        if (variant == null)
            throw new ArgumentException("Variant not found");

        variant.StockQuantity += quantityChange;
        await _variantRepository.UpdateAsync(variant);
    }
    public async Task<IEnumerable<ProductVariantDto>> GetAllVariantsAsync(Guid productId)
    {
        var variants = await _variantRepository.GetAllByProductIdAsync(productId);
        return variants.Select(v => new ProductVariantDto
        {
            Id = v.Id,
            ProductId = v.ProductId,
            Size = v.Size,
            Color = v.Color,
            StockQuantity = v.StockQuantity,
            AdditionalPrice = v.AdditionalPrice
        });
    }
    private ProductVariantDto MapToDto(ProductVariant variant)
    {
        return new ProductVariantDto
        {
            Id = variant.Id,
            ProductId = variant.ProductId,
            Size = variant.Size,
            Color = variant.Color,
            StockQuantity = variant.StockQuantity,
            AdditionalPrice = variant.AdditionalPrice
        };
    }
}