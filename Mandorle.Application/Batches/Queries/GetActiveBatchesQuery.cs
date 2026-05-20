using Mandorle.Application.Batches.Models;
using MediatR;

namespace Mandorle.Application.Batches.Queries;

public record GetActiveBatchesQuery(string? Search) : IRequest<IReadOnlyList<ActiveBatchRowDto>>;
