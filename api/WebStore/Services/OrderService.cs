using WebStore.DTOs;
using WebStore.Models;
using WebStore.Repositories;

namespace WebStore.Services;

public class OrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUserRepository _userRepository;
    private readonly IProductVariantRepository _productVariantRepository;

    public OrderService(
        IOrderRepository orderRepository,
        IUserRepository userRepository,
        IProductVariantRepository productVariantRepository)
    {
        _orderRepository = orderRepository;
        _userRepository = userRepository;
        _productVariantRepository = productVariantRepository;
    }

    public async Task<OrderDto> CreateOrderAsync(Guid userId, CreateOrderDto dto)
    {
        var user = await _userRepository.GetUserByIdAsync(userId);
        if (user == null) throw new ArgumentException("User not found");

        var order = new Order
        {
            UserId = userId,
            Status = OrderStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        decimal totalAmount = 0;
        foreach (var item in dto.Items)
        {
            var variant = await _productVariantRepository.GetByIdAsync(item.ProductVariantId);
            if (variant == null)
                throw new ArgumentException($"Product variant not found: {item.ProductVariantId}");

            if (!await _productVariantRepository.IsInStockAsync(item.ProductVariantId, item.Quantity))
                throw new ArgumentException($"Not enough stock for variant: {item.ProductVariantId}");

            var orderItem = new OrderItem
            {
                ProductVariantId = item.ProductVariantId,
                Quantity = item.Quantity,
                PriceAtPurchase = variant.Product.Price + variant.AdditionalPrice
            };

            totalAmount += orderItem.PriceAtPurchase * item.Quantity;
            order.OrderItems.Add(orderItem);

            variant.StockQuantity -= item.Quantity;
            await _productVariantRepository.UpdateAsync(variant);
        }

        order.TotalAmount = totalAmount;
        var createdOrder = await _orderRepository.CreateAsync(order);
        return MapToDto(createdOrder);
    }

    public async Task<OrderDto?> GetOrderByIdAsync(Guid orderId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        return order != null ? MapToDto(order) : null;
    }

    public async Task<IEnumerable<OrderDto>> GetUserOrdersAsync(Guid userId)
    {
        var orders = await _orderRepository.GetByUserIdAsync(userId);
        return orders.Select(MapToDto);
    }

    public async Task<OrderDto?> UpdateOrderStatusAsync(Guid orderId, UpdateOrderStatusDto dto)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order == null) return null;

        order.Status = dto.Status;
        order.UpdatedAt = DateTime.UtcNow;

        var updatedOrder = await _orderRepository.UpdateAsync(order);
        return MapToDto(updatedOrder);
    }

    private OrderDto MapToDto(Order order)
    {
        return new OrderDto
        {
            Id = order.Id,
            UserId = order.UserId,
            TotalAmount = order.TotalAmount,
            Status = order.Status.ToString(),
            CreatedAt = order.CreatedAt,
            Items = order.OrderItems.Select(oi => new OrderItemDetailDto
            {
                ProductVariantId = oi.ProductVariantId,
                ProductName = oi.ProductVariant.Product.Name,
                VariantInfo = $"{oi.ProductVariant.Size} / {oi.ProductVariant.Color}",
                Quantity = oi.Quantity,
                UnitPrice = oi.PriceAtPurchase,
                TotalPrice = oi.PriceAtPurchase * oi.Quantity
            }).ToList()
        };
    }
}