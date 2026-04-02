namespace Mandorle.Application.Orders.Models;

public record OrderDto(
    int Id,
    int CustomerId,
    string OrderType,
    string Status,
    string? PaymentStatus,
    decimal TotalAmount,
    string Currency,
    string? Notes,
    IReadOnlyList<OrderItemDto> Items);
