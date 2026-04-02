using Mandorle.Application.StockReservations.Models;
using Mandorle.Domain.Entities;

namespace Mandorle.Application.StockReservations.Mapping;

internal static class StockReservationMappings
{
    public static StockReservationDto ToDto(this StockReservation reservation)
    {
        return new StockReservationDto(
            reservation.Id,
            reservation.OrderId,
            reservation.OrderItemId,
            reservation.ProductId,
            reservation.BatchId,
            reservation.Quantity,
            reservation.Status,
            reservation.ReservationType,
            reservation.ExpiresAt,
            reservation.ReleasedAt,
            reservation.ConsumedAt,
            reservation.Notes);
    }
}
