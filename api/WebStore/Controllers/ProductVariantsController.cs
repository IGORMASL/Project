using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebStore.DTOs;
using WebStore.Services;

namespace WebStore.Controllers;

[ApiController]
[Route("api/products/{productId}/variants")]
public class ProductVariantsController : ControllerBase
{
    private readonly ProductVariantService _variantService;

    public ProductVariantsController(ProductVariantService variantService)
    {
        _variantService = variantService;
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateVariant(Guid productId, [FromBody] ProductVariantDto dto)
    {
        try
        {
            var variant = await _variantService.CreateVariantAsync(productId, dto);
            return CreatedAtAction(nameof(GetVariantById), new { productId, id = variant.Id }, variant);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetVariantById(Guid productId, Guid id)
    {
        try
        {
            var variant = await _variantService.GetByIdAsync(id);
            return Ok(variant);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateVariant(Guid productId, Guid id, [FromBody] ProductVariantDto dto)
    {
        try
        {
            var variant = await _variantService.UpdateVariantAsync(id, dto);
            return Ok(variant);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteVariant(Guid productId, Guid id)
    {
        try
        {
            await _variantService.DeleteVariantAsync(id);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
    }
}