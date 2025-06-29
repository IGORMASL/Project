using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebStore.DTOs;
using WebStore.Services;

namespace WebStore.Controllers;

[ApiController]
[Route("api/products/{productId}/reviews")]
public class ReviewsController : ControllerBase
{
    private readonly ReviewService _reviewService;

    public ReviewsController(ReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateReview(Guid productId, [FromBody] CreateReviewDto dto)
    {
        var userId = GetCurrentUserId();
        try
        {
            var review = await _reviewService.CreateReviewAsync(userId, productId, dto);
            return CreatedAtAction(nameof(GetReviewById), new { productId, id = review.Id }, review);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetReviewById(Guid productId, Guid id)
    {
        var review = await _reviewService.GetByIdAsync(id);
        return review == null ? NotFound() : Ok(review);
    }

    [HttpGet]
    public async Task<IActionResult> GetProductReviews(Guid productId)
    {
        var reviews = await _reviewService.GetProductReviewsAsync(productId);
        return Ok(reviews);
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateReview(Guid productId, Guid id, [FromBody] UpdateReviewDto dto)
    {
        var userId = GetCurrentUserId();
        var review = await _reviewService.UpdateReviewAsync(id, userId, dto);
        return review == null ? NotFound() : Ok(review);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteReview(Guid productId, Guid id)
    {
        var userId = GetCurrentUserId();
        var result = await _reviewService.DeleteReviewAsync(id, userId);
        return result ? NoContent() : NotFound();
    }

    [HttpGet("can-review")]
    [Authorize]
    public async Task<IActionResult> CanUserReview(Guid productId)
    {
        var userId = GetCurrentUserId();
        var canReview = await _reviewService.CanUserReviewProduct(userId, productId);
        return Ok(new { canReview });
    }

    private Guid GetCurrentUserId()
    {
        return Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
    }
}