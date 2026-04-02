using Mandorle.Domain.Entities;

namespace Mandorle.Domain.Interfaces;

public interface IPublicTraceViewRepository
{
    Task<PublicTraceView?> GetByBatchIdAsync(int batchId, CancellationToken cancellationToken = default);

    Task<PublicTraceView?> GetByBatchCodeAsync(string batchCode, CancellationToken cancellationToken = default);

    Task AddAsync(PublicTraceView publicTraceView, CancellationToken cancellationToken = default);

    void Update(PublicTraceView publicTraceView);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
