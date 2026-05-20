namespace Mandorle.Application.Batches.Models;

public record BatchParentDetailDto(
    int BatchLinkId,
    int ParentBatchId,
    string ParentBatchCode,
    int ProductId,
    string BatchType,
    string? Variety,
    bool BioFlag,
    decimal QuantityUsed,
    string UnitOfMeasure,
    int? SupplierId);
