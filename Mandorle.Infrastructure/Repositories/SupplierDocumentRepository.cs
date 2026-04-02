using Mandorle.Domain.Entities;
using Mandorle.Domain.Interfaces;
using Mandorle.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Mandorle.Infrastructure.Repositories;

public class SupplierDocumentRepository : ISupplierDocumentRepository
{
    private readonly StellaFruttaDbContext _context;

    public SupplierDocumentRepository(StellaFruttaDbContext context)
    {
        _context = context;
    }

    public Task<SupplierDocument?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _context.SupplierDocuments.FirstOrDefaultAsync(document => document.Id == id, cancellationToken);
    }

    public Task<SupplierDocument?> GetByIdAsync(int supplierId, int id, CancellationToken cancellationToken = default)
    {
        return _context.SupplierDocuments.FirstOrDefaultAsync(
            document => document.SupplierId == supplierId && document.Id == id,
            cancellationToken);
    }

    public async Task<IReadOnlyList<SupplierDocument>> GetBySupplierIdAsync(int supplierId, CancellationToken cancellationToken = default)
    {
        return await _context.SupplierDocuments
            .AsNoTracking()
            .Where(document => document.SupplierId == supplierId)
            .OrderByDescending(document => document.UploadedAt)
            .ThenByDescending(document => document.Id)
            .ToListAsync(cancellationToken);
    }

    public Task AddAsync(SupplierDocument document, CancellationToken cancellationToken = default)
    {
        NormalizeDocument(document);
        return _context.SupplierDocuments.AddAsync(document, cancellationToken).AsTask();
    }

    public void Update(SupplierDocument document)
    {
        NormalizeDocument(document);
        _context.SupplierDocuments.Update(document);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }

    private static void NormalizeDocument(SupplierDocument document)
    {
        document.DocumentType = Normalize(document.DocumentType)!;
        document.FileName = Normalize(document.FileName)!;
        document.StoragePath = Normalize(document.StoragePath)!;
        document.UploadedBy = Normalize(document.UploadedBy);
    }

    private static string? Normalize(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
