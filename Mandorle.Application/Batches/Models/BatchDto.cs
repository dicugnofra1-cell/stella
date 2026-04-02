namespace Mandorle.Application.Batches.Models;

public record BatchDto(
    int Id,
    string BatchCode,
    int ProductId,
    string BatchType,
    string Status,
    bool BioFlag,
    int? SupplierId,
    DateOnly? ProductionDate,
    DateOnly? ExpirationDate,
    string? Notes);
