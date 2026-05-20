using Mandorle.Application.Batches.Models;
using MediatR;

namespace Mandorle.Application.Batches.Commands;

public record RegisterBatchProcessingCommand(
    string ProcessingType,
    string ResultBatchType,
    decimal ResultQuantity,
    string UnitOfMeasure,
    string? Notes,
    string UserId,
    IReadOnlyList<RegisterBatchProcessingSourceInput> Sources) : IRequest<BatchProcessingResultDto>;

public record RegisterBatchProcessingSourceInput(
    int BatchId,
    decimal Quantity);
