namespace Mandorle.Application.Batches.Models;

public record BatchProcessingResultDto(
    BatchDto Batch,
    IReadOnlyList<BatchLinkDto> Parents,
    decimal ResultQuantity,
    string UnitOfMeasure);
