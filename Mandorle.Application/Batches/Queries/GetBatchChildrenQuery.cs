using Mandorle.Application.Batches.Models;
using MediatR;

namespace Mandorle.Application.Batches.Queries;

public record GetBatchChildrenQuery(int ParentBatchId) : IRequest<IReadOnlyList<BatchLinkDto>>;
