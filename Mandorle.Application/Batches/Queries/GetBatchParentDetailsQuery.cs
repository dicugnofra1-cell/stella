using Mandorle.Application.Batches.Models;
using MediatR;

namespace Mandorle.Application.Batches.Queries;

public record GetBatchParentDetailsQuery(int ChildBatchId) : IRequest<IReadOnlyList<BatchParentDetailDto>>;
