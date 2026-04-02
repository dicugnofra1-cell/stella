using Mandorle.Application.Orders.Models;
using Mandorle.Domain.Entities;

namespace Mandorle.Application.Orders.Mapping;

internal static class OrderMappings
{
    public static OrderDto ToDto(this Order order)
    {
        return new OrderDto(
            order.Id,
            order.CustomerId,
            order.OrderType,
            order.Status,
            order.PaymentStatus,
            order.TotalAmount,
            order.Currency,
            order.Notes,
            order.OrderItems
                .OrderBy(orderItem => orderItem.Id)
                .Select(orderItem => orderItem.ToDto())
                .ToList());
    }

    public static OrderItemDto ToDto(this OrderItem orderItem)
    {
        return new OrderItemDto(
            orderItem.Id,
            orderItem.OrderId,
            orderItem.ProductId,
            orderItem.Quantity,
            orderItem.UnitPrice,
            orderItem.TaxAmount,
            orderItem.ReservedBatchId);
    }
}
