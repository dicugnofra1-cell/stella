namespace Mandorle.Application.Orders.Models;

public record OrderItemDto(
    int Id,
    int OrderId,
    int ProductId,
    decimal Quantity,
    decimal UnitPrice,
    decimal? TaxAmount,
    int? ReservedBatchId);
