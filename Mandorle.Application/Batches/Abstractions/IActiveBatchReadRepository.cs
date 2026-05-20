using Mandorle.Application.Batches.Models;

namespace Mandorle.Application.Batches.Abstractions;

public interface IActiveBatchReadRepository
{
    Task<IReadOnlyList<ActiveBatchRowDto>> SearchActiveAsync(string? search, CancellationToken cancellationToken = default);
}
