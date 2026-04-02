using Mandorle.Application.Orders.Models;
using MediatR;

namespace Mandorle.Application.Orders.Commands;

public record UpdateOrderCommand(
    int Id,
    string OrderType,
    string Status,
    string? PaymentStatus,
    decimal TotalAmount,
    string Currency,
    string? Notes) : IRequest<OrderDto?>;
