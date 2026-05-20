namespace Mandorle.Application.Batches.Models;

public record BatchDto(
    int Id,
    string BatchCode,
    int ProductId,
    string BatchType,
    string Status,
    bool BioFlag,
    string? Variety,
    decimal? InitialQuantity,
    decimal? PurchaseUnitPrice,
    string? UnitOfMeasure,
    int? SupplierId,
    int? SupplierDocumentId,
    int? CertificationId,
    DateOnly? ProductionDate,
    DateOnly? ExpirationDate,
    string? Notes,
    DateTime CreatedAt);
