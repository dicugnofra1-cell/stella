namespace Mandorle.Application.GoodsReceipts.Models;

public record GoodsReceiptRowDto(
    int BatchId,
    string BatchCode,
    DateTime CreatedAt,
    string SupplierName,
    string ProductName,
    string BatchType,
    bool BioFlag,
    decimal Quantity,
    string UnitOfMeasure,
    string Status,
    string? Variety,
    string? Notes,
    string Operator,
    int? SupplierId,
    int? CertificationId);
