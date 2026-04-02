using Mandorle.Domain.Entities;

namespace Mandorle.Domain.Interfaces;

public interface IBatchRepository
{
    Task<Batch?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<Batch?> GetByBatchCodeAsync(string batchCode, CancellationToken cancellationToken = default);

    Task<bool> ExistsByBatchCodeAsync(string batchCode, int? excludeBatchId = null, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Batch>> SearchAsync(
        string? search,
        int? productId,
        int? supplierId,
        string? batchType,
        string? status,
        bool? bioFlag,
        CancellationToken cancellationToken = default);

    Task AddAsync(Batch batch, CancellationToken cancellationToken = default);

    void Update(Batch batch);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
