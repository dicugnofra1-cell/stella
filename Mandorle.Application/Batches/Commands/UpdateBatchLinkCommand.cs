using Mandorle.Application.Batches.Models;
using MediatR;

namespace Mandorle.Application.Batches.Commands;

public record UpdateBatchLinkCommand(
    int ChildBatchId,
    int BatchLinkId,
    int ParentBatchId,
    decimal QuantityUsed,
    string UnitOfMeasure) : IRequest<BatchLinkDto?>;
