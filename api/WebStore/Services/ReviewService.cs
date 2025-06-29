using WebStore.DTOs;
using WebStore.Models;
using WebStore.Repositories;

namespace WebStore.Services;

public class ReviewService
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IUserRepository _userRepository;
    private readonly IProductRepository _productRepository;

    public ReviewService(
        IReviewRepository reviewRepository,
        IUserRepository userRepository,
        IProductRepository productRepository)
    {
        _reviewRepository = reviewRepository;
        _userRepository = userRepository;
        _productRepository = productRepository;
    }

    public async Task<ReviewDto?> GetByIdAsync(Guid id)
    {
        var review = await _reviewRepository.GetByIdAsync(id);
        if (review == null) return null;

        var user = await _userRepository.GetUserByIdAsync(review.UserId);
        if (user == null) return null;

        return MapToDto(review, user);
    }

    public async Task<ReviewDto> CreateReviewAsync(Guid userId, Guid productId, CreateReviewDto dto)
    {
        var user = await _userRepository.GetUserByIdAsync(userId);
        if (user == null)
            throw new ArgumentException("User not found");

        var product = await _productRepository.GetByIdAsync(productId);
        if (product == null)
            throw new ArgumentException("Product not found");

        if (await _reviewRepository.HasUserReviewedProduct(userId, productId))
            throw new InvalidOperationException("User has already reviewed this product");

        var review = new Review
        {
            UserId = userId,
            ProductId = productId,
            Rating = dto.Rating,
            Comment = dto.Comment,
            CreatedAt = DateTime.UtcNow
        };

        var createdReview = await _reviewRepository.CreateAsync(review);
        return MapToDto(createdReview, user);
    }

    public async Task<ReviewDto?> UpdateReviewAsync(Guid reviewId, Guid userId, UpdateReviewDto dto)
    {
        var review = await _reviewRepository.GetByIdAsync(reviewId);
        if (review == null || review.UserId != userId)
            return null;

        if (dto.Rating.HasValue)
            review.Rating = dto.Rating.Value;

        if (dto.Comment != null)
            review.Comment = dto.Comment;

        var updatedReview = await _reviewRepository.UpdateAsync(review);
        var user = await _userRepository.GetUserByIdAsync(userId);
        return user == null ? null : MapToDto(updatedReview, user);
    }

    public async Task<bool> DeleteReviewAsync(Guid reviewId, Guid userId)
    {
        var review = await _reviewRepository.GetByIdAsync(reviewId);
        if (review == null || review.UserId != userId)
            return false;

        return await _reviewRepository.DeleteAsync(reviewId);
    }

    public async Task<IEnumerable<ReviewDto>> GetProductReviewsAsync(Guid productId)
    {
        var reviews = await _reviewRepository.GetByProductIdAsync(productId);
        var result = new List<ReviewDto>();

        foreach (var review in reviews)
        {
            var user = await _userRepository.GetUserByIdAsync(review.UserId);
            if (user != null)
            {
                result.Add(MapToDto(review, user));
            }
        }

        return result;
    }

    public async Task<IEnumerable<ReviewDto>> GetUserReviewsAsync(Guid userId)
    {
        var reviews = await _reviewRepository.GetByUserIdAsync(userId);
        var user = await _userRepository.GetUserByIdAsync(userId);

        if (user == null) return Enumerable.Empty<ReviewDto>();

        return reviews.Select(r => MapToDto(r, user));
    }

    public async Task<bool> CanUserReviewProduct(Guid userId, Guid productId)
    {
        return !await _reviewRepository.HasUserReviewedProduct(userId, productId);
    }

    private ReviewDto MapToDto(Review review, User user)
    {
        return new ReviewDto
        {
            Id = review.Id,
            UserId = user.Id,
            UserName = user.FullName,
            ProductId = review.ProductId,
            Rating = review.Rating,
            Comment = review.Comment,
            CreatedAt = review.CreatedAt
        };
    }
}