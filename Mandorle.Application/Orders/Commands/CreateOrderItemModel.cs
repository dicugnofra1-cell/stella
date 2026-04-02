namespace Mandorle.Application.Orders.Commands;

public record CreateOrderItemModel(
    int ProductId,
    decimal Quantity,
    decimal UnitPrice,
    decimal? TaxAmount,
    int? ReservedBatchId);
