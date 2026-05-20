using Mandorle.Domain.Entities;
using Mandorle.Domain.Interfaces;
using Mandorle.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Mandorle.Infrastructure.Repositories;

public class InvoiceRepository : IInvoiceRepository
{
    private readonly StellaFruttaDbContext _context;

    public InvoiceRepository(StellaFruttaDbContext context)
    {
        _context = context;
    }

    public Task<Invoice?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _context.Set<Invoice>().FirstOrDefaultAsync(invoice => invoice.Id == id, cancellationToken);
    }

    public Task<Invoice?> GetByOrderIdAsync(int orderId, CancellationToken cancellationToken = default)
    {
        return _context.Set<Invoice>().FirstOrDefaultAsync(invoice => invoice.OrderId == orderId, cancellationToken);
    }

    public Task<bool> ExistsByDocumentNumberAsync(string documentNumber, int? excludeInvoiceId = null, CancellationToken cancellationToken = default)
    {
        var normalizedDocumentNumber = Normalize(documentNumber);

        return _context.Set<Invoice>().AnyAsync(
            invoice => invoice.DocumentNumber == normalizedDocumentNumber && (!excludeInvoiceId.HasValue || invoice.Id != excludeInvoiceId.Value),
            cancellationToken);
    }

    public async Task<IReadOnlyList<Invoice>> SearchAsync(
        int? orderId,
        int? customerId,
        string? documentType,
        DateTime? fromDate,
        DateTime? toDate,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Set<Invoice>().AsNoTracking().AsQueryable();
        var normalizedDocumentType = Normalize(documentType);

        if (orderId.HasValue)
        {
            query = query.Where(invoice => invoice.OrderId == orderId.Value);
        }

        if (customerId.HasValue)
        {
            query = query.Where(invoice => invoice.CustomerId == customerId.Value);
        }

        if (!string.IsNullOrWhiteSpace(normalizedDocumentType))
        {
            query = query.Where(invoice => invoice.DocumentType == normalizedDocumentType);
        }

        if (fromDate.HasValue)
        {
            query = query.Where(invoice => invoice.IssueDate >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            query = query.Where(invoice => invoice.IssueDate <= toDate.Value);
        }

        return await query
            .OrderByDescending(invoice => invoice.IssueDate)
            .ThenByDescending(invoice => invoice.Id)
            .ToListAsync(cancellationToken);
    }

    public Task AddAsync(Invoice invoice, CancellationToken cancellationToken = default)
    {
        NormalizeInvoice(invoice);
        return _context.Set<Invoice>().AddAsync(invoice, cancellationToken).AsTask();
    }

    public void Update(Invoice invoice)
    {
        NormalizeInvoice(invoice);
        _context.Set<Invoice>().Update(invoice);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }

    private static void NormalizeInvoice(Invoice invoice)
    {
        invoice.DocumentNumber = Normalize(invoice.DocumentNumber)!;
        invoice.DocumentType = Normalize(invoice.DocumentType)!;
        invoice.Currency = Normalize(invoice.Currency)!;
        invoice.Source = Normalize(invoice.Source)!;
        invoice.SyncStatus = Normalize(invoice.SyncStatus)!;
        invoice.ExternalProvider = Normalize(invoice.ExternalProvider);
        invoice.ExternalDocumentId = Normalize(invoice.ExternalDocumentId);
        invoice.ExternalDocumentNumber = Normalize(invoice.ExternalDocumentNumber);
        invoice.ExternalSyncError = Normalize(invoice.ExternalSyncError);
        invoice.Notes = Normalize(invoice.Notes);
    }

    private static string? Normalize(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim().ToUpperInvariant();
    }
}
