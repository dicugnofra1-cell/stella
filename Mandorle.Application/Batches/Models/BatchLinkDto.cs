namespace Mandorle.Application.Batches.Models;

public record BatchLinkDto(
    int Id,
    int ParentBatchId,
    int ChildBatchId,
    decimal QuantityUsed,
    string UnitOfMeasure);
