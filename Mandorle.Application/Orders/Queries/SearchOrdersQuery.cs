using Mandorle.Application.Orders.Models;
using MediatR;

namespace Mandorle.Application.Orders.Queries;

public record SearchOrdersQuery(
    int? CustomerId,
    string? OrderType,
    string? Status,
    string? PaymentStatus,
    DateTime? FromDate,
    DateTime? ToDate) : IRequest<IReadOnlyList<OrderDto>>;
