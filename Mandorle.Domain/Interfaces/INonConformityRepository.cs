using Mandorle.Domain.Entities;

namespace Mandorle.Domain.Interfaces;

public interface INonConformityRepository
{
    Task<NonConformity?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<NonConformity>> SearchAsync(
        int? batchId,
        string? severity,
        string? status,
        CancellationToken cancellationToken = default);

    Task AddAsync(NonConformity nonConformity, CancellationToken cancellationToken = default);

    void Update(NonConformity nonConformity);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
