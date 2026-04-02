using Mandorle.Application.Batches.Models;
using MediatR;

namespace Mandorle.Application.Batches.Commands;

public record AddBatchLinkCommand(
    int ParentBatchId,
    int ChildBatchId,
    decimal QuantityUsed,
    string UnitOfMeasure) : IRequest<BatchLinkDto>;
