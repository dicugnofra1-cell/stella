namespace Mandorle.Application.Batches.Models;

public record ActiveBatchRowDto(
    int BatchId,
    string BatchCode,
    int ProductId,
    string ProductName,
    string BatchType,
    string Status,
    bool BioFlag,
    string? Variety,
    decimal PhysicalStock,
    decimal ReservedStock,
    decimal AvailableStock,
    string UnitOfMeasure,
    string SupplierName,
    int? SupplierId,
    bool QrActive,
    DateTime CreatedAt);
