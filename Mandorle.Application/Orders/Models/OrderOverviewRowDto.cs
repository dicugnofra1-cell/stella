namespace Mandorle.Application.Orders.Models;

public record OrderOverviewRowDto(
    int OrderId,
    string OrderCode,
    DateTime CreatedAt,
    int CustomerId,
    string CustomerName,
    string CustomerType,
    string OrderType,
    string Status,
    string? PaymentStatus,
    string? LotCodes,
    decimal TotalAmount,
    string Currency,
    int ItemCount,
    string? InvoiceDocumentNumber,
    string? InvoiceDocumentType);
