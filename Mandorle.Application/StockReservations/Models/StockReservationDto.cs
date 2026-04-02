namespace Mandorle.Application.StockReservations.Models;

public record StockReservationDto(
    int Id,
    int OrderId,
    int OrderItemId,
    int ProductId,
    int? BatchId,
    decimal Quantity,
    string Status,
    string ReservationType,
    DateTime? ExpiresAt,
    DateTime? ReleasedAt,
    DateTime? ConsumedAt,
    string? Notes);
