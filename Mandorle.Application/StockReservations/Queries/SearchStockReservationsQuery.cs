using Mandorle.Application.StockReservations.Models;
using MediatR;

namespace Mandorle.Application.StockReservations.Queries;

public record SearchStockReservationsQuery(
    int? OrderId,
    int? OrderItemId,
    int? ProductId,
    int? BatchId,
    string? Status,
    string? ReservationType) : IRequest<IReadOnlyList<StockReservationDto>>;
