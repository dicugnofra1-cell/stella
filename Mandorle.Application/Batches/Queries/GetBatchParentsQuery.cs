using Mandorle.Application.Batches.Models;
using MediatR;

namespace Mandorle.Application.Batches.Queries;

public record GetBatchParentsQuery(int ChildBatchId) : IRequest<IReadOnlyList<BatchLinkDto>>;
