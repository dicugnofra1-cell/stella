using Mandorle.Domain.Entities;

namespace Mandorle.Domain.Interfaces;

public interface ICertificationRepository
{
    Task<Certification?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<Certification?> GetByIdAsync(int supplierId, int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Certification>> GetBySupplierIdAsync(
        int supplierId,
        string? type = null,
        string? status = null,
        CancellationToken cancellationToken = default);

    Task AddAsync(Certification certification, CancellationToken cancellationToken = default);

    void Update(Certification certification);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
