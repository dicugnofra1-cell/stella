using Mandorle.Application.StockReservations.Models;
using MediatR;

namespace Mandorle.Application.StockReservations.Commands;

public record CreateStockReservationCommand(
    int OrderId,
    int OrderItemId,
    int ProductId,
    int? BatchId,
    decimal Quantity,
    string Status,
    string ReservationType,
    DateTime? ExpiresAt,
    string? Notes) : IRequest<StockReservationDto>;
