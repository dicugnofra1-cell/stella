using Mandorle.Application.Orders.Models;
using MediatR;

namespace Mandorle.Application.Orders.Commands;

public record CreateOrderCommand(
    int? CustomerId,
    CreateOrderCustomerInput? NewCustomer,
    string OrderType,
    string Status,
    string? PaymentStatus,
    decimal TotalAmount,
    string Currency,
    string? Notes,
    IReadOnlyList<CreateOrderItemModel> Items) : IRequest<OrderDto>;

public record CreateOrderCustomerInput(
    string Type,
    string Name,
    string Email,
    string? VatNumber,
    string? Pec,
    string? SdiCode,
    string? SpidIdentifier,
    string? Phone,
    string? Status);
