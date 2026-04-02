using Mandorle.Domain.Entities;

namespace Mandorle.Domain.Interfaces;

public interface ISupplierDocumentRepository
{
    Task<SupplierDocument?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<SupplierDocument?> GetByIdAsync(int supplierId, int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<SupplierDocument>> GetBySupplierIdAsync(int supplierId, CancellationToken cancellationToken = default);

    Task AddAsync(SupplierDocument document, CancellationToken cancellationToken = default);

    void Update(SupplierDocument document);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
