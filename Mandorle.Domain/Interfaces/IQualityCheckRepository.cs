using Mandorle.Domain.Entities;

namespace Mandorle.Domain.Interfaces;

public interface IQualityCheckRepository
{
    Task<QualityCheck?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<QualityCheck>> GetByBatchIdAsync(int batchId, CancellationToken cancellationToken = default);

    Task AddAsync(QualityCheck qualityCheck, CancellationToken cancellationToken = default);

    void Update(QualityCheck qualityCheck);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
