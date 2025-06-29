using Microsoft.AspNetCore.Hosting;
using WebStore.Data;
using WebStore.DTOs;
using WebStore.Models;
using WebStore.Repositories;

namespace WebStore.Services;

public class ProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IWebHostEnvironment _environment;
    private readonly IConfiguration _configuration;

    public ProductService(
        IProductRepository productRepository,
        IWebHostEnvironment environment,
        IConfiguration configuration)
    {
        _productRepository = productRepository;
        _environment = environment;
        _configuration = configuration;
    }

    public async Task<ProductDto> CreateProductAsync(CreateProductDto dto, IFormFile? imageFile)
    {
        var product = new Product
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price
        };

        if (imageFile != null)
        {
            product.ImagePath = await SaveImageAsync(imageFile);
        }

        var createdProduct = await _productRepository.CreateAsync(product);
        return MapToDto(createdProduct);
    }

    public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
    {
        var products = await _productRepository.GetAllAsync();
        return products.Select(MapToDto);
    }

    public async Task<ProductDto?> GetProductByIdAsync(Guid id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        return product != null ? MapToDto(product) : null;
    }

    public async Task<ProductDto?> UpdateProductAsync(Guid id, UpdateProductDto dto, IFormFile? imageFile)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null) return null;

        if (!string.IsNullOrEmpty(dto.Name)) product.Name = dto.Name;
        if (!string.IsNullOrEmpty(dto.Description)) product.Description = dto.Description;
        if (dto.Price.HasValue) product.Price = dto.Price.Value;

        if (imageFile != null)
        {
            if (!string.IsNullOrEmpty(product.ImagePath))
            {
                DeleteImage(product.ImagePath);
            }
            product.ImagePath = await SaveImageAsync(imageFile);
        }

        var updatedProduct = await _productRepository.UpdateAsync(product);
        return MapToDto(updatedProduct);
    }

    public async Task<bool> DeleteProductAsync(Guid id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null) return false;

        if (!string.IsNullOrEmpty(product.ImagePath))
        {
            DeleteImage(product.ImagePath);
        }

        return await _productRepository.DeleteAsync(id);
    }

    private async Task<string> SaveImageAsync(IFormFile imageFile)
    {
        var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "products");
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await imageFile.CopyToAsync(fileStream);
        }

        return Path.Combine("uploads", "products", uniqueFileName);
    }

    private void DeleteImage(string imagePath)
    {
        var fullPath = Path.Combine(_environment.WebRootPath, imagePath);
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }
    }

    private ProductDto MapToDto(Product product)
    {
        var baseUrl = _configuration["BaseUrl"];
        var imageUrl = !string.IsNullOrEmpty(product.ImagePath)
            ? $"{baseUrl}/{product.ImagePath.Replace('\\', '/')}"
            : null;

        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            ImageUrl = imageUrl
        };
    }
    public async Task<ProductVariantDto?> GetVariantByIdAsync(Guid variantId)
    {
        var variant = await _productRepository.GetVariantByIdAsync(variantId);
        if (variant == null) return null;

        var product = await GetProductByIdAsync(variant.ProductId);
        if (product == null) return null;

        return new ProductVariantDto
        {
            Id = variant.Id,
            StockQuantity = variant.StockQuantity,
            AdditionalPrice = variant.AdditionalPrice,
            Product = product
        };
    }
}