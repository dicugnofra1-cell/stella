using Mandorle.Application.Quality.Models;
using MediatR;

namespace Mandorle.Application.Quality.Queries;

public record GetQualityChecksByBatchQuery(int BatchId) : IRequest<IReadOnlyList<QualityCheckDto>>;
