using Mandorle.Application.Orders.Models;
using MediatR;

namespace Mandorle.Application.Orders.Commands;

public record CreateOrderCommand(
    int CustomerId,
    string OrderType,
    string Status,
    string? PaymentStatus,
    decimal TotalAmount,
    string Currency,
    string? Notes,
    IReadOnlyList<CreateOrderItemModel> Items) : IRequest<OrderDto>;
