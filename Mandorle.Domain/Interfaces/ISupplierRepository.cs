using Mandorle.Domain.Entities;

namespace Mandorle.Domain.Interfaces;

public interface ISupplierRepository
{
    Task<Supplier?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<Supplier?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    Task<Supplier?> GetByVatNumberAsync(string vatNumber, CancellationToken cancellationToken = default);

    Task<bool> ExistsByEmailAsync(string? email, int? excludeSupplierId = null, CancellationToken cancellationToken = default);

    Task<bool> ExistsByVatNumberAsync(string? vatNumber, int? excludeSupplierId = null, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Supplier>> SearchAsync(string? search, string? status, CancellationToken cancellationToken = default);

    Task AddAsync(Supplier supplier, CancellationToken cancellationToken = default);

    void Update(Supplier supplier);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
