using Mandorle.Domain.Entities;

namespace Mandorle.Domain.Interfaces;

public interface IInvoiceRepository
{
    Task<Invoice?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<Invoice?> GetByOrderIdAsync(int orderId, CancellationToken cancellationToken = default);

    Task<bool> ExistsByDocumentNumberAsync(string documentNumber, int? excludeInvoiceId = null, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Invoice>> SearchAsync(
        int? orderId,
        int? customerId,
        string? documentType,
        DateTime? fromDate,
        DateTime? toDate,
        CancellationToken cancellationToken = default);

    Task AddAsync(Invoice invoice, CancellationToken cancellationToken = default);

    void Update(Invoice invoice);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
