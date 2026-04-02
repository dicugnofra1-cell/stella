using Mandorle.Domain.Entities;

namespace Mandorle.Domain.Interfaces;

public interface IBatchLinkRepository
{
    Task<BatchLink?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<BatchLink?> GetByIdAsync(int childBatchId, int id, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(int parentBatchId, int childBatchId, int? excludeBatchLinkId = null, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<BatchLink>> GetByChildBatchIdAsync(int childBatchId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<BatchLink>> GetByParentBatchIdAsync(int parentBatchId, CancellationToken cancellationToken = default);

    Task AddAsync(BatchLink batchLink, CancellationToken cancellationToken = default);

    void Update(BatchLink batchLink);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
