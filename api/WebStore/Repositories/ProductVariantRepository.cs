using Microsoft.EntityFrameworkCore;
using WebStore.Data;
using WebStore.Models;

namespace WebStore.Repositories;

public class ProductVariantRepository : IProductVariantRepository
{
    private readonly AppDbContext _context;

    public ProductVariantRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ProductVariant?> GetByIdAsync(Guid id)
    {
        return await _context.ProductVariants
            .Include(pv => pv.Product)
            .FirstOrDefaultAsync(pv => pv.Id == id);
    }

    public async Task<ProductVariant> CreateAsync(ProductVariant variant)
    {
        _context.ProductVariants.Add(variant);
        await _context.SaveChangesAsync();
        return variant;
    }

    public async Task<ProductVariant> UpdateAsync(ProductVariant variant)
    {
        _context.ProductVariants.Update(variant);
        await _context.SaveChangesAsync();
        return variant;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var variant = await _context.ProductVariants.FindAsync(id);
        if (variant == null) return false;

        _context.ProductVariants.Remove(variant);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> IsInStockAsync(Guid variantId, int quantity)
    {
        var variant = await _context.ProductVariants
            .FirstOrDefaultAsync(pv => pv.Id == variantId);

        return variant?.StockQuantity >= quantity;
    }
}