namespace Mandorle.Application.Batches.Models;

public record BatchSaleSuggestionDto(
    int BatchId,
    string BatchCode,
    int ProductId,
    string BatchType,
    bool BioFlag,
    decimal PhysicalStock,
    decimal ReservedStock,
    decimal AvailableStock);
