using Mandorle.Domain.Entities;
using Mandorle.Domain.Interfaces;
using Mandorle.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Mandorle.Infrastructure.Repositories;

public class BatchRepository : IBatchRepository
{
    private readonly StellaFruttaDbContext _context;

    public BatchRepository(StellaFruttaDbContext context)
    {
        _context = context;
    }

    public Task<Batch?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _context.Batches.FirstOrDefaultAsync(batch => batch.Id == id, cancellationToken);
    }

    public Task<Batch?> GetByBatchCodeAsync(string batchCode, CancellationToken cancellationToken = default)
    {
        var normalizedBatchCode = Normalize(batchCode);
        return _context.Batches.FirstOrDefaultAsync(batch => batch.BatchCode == normalizedBatchCode, cancellationToken);
    }

    public Task<bool> ExistsByBatchCodeAsync(string batchCode, int? excludeBatchId = null, CancellationToken cancellationToken = default)
    {
        var normalizedBatchCode = Normalize(batchCode);

        return _context.Batches.AnyAsync(
            batch => batch.BatchCode == normalizedBatchCode && (!excludeBatchId.HasValue || batch.Id != excludeBatchId.Value),
            cancellationToken);
    }

    public async Task<IReadOnlyList<Batch>> SearchAsync(
        string? search,
        int? productId,
        int? supplierId,
        string? batchType,
        string? status,
        bool? bioFlag,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Batches.AsNoTracking().AsQueryable();
        var normalizedSearch = Normalize(search);
        var normalizedBatchType = Normalize(batchType);
        var normalizedStatus = Normalize(status);

        if (productId.HasValue)
        {
            query = query.Where(batch => batch.ProductId == productId.Value);
        }

        if (supplierId.HasValue)
        {
            query = query.Where(batch => batch.SupplierId == supplierId.Value);
        }

        if (!string.IsNullOrWhiteSpace(normalizedBatchType))
        {
            query = query.Where(batch => batch.BatchType == normalizedBatchType);
        }

        if (!string.IsNullOrWhiteSpace(normalizedStatus))
        {
            query = query.Where(batch => batch.Status == normalizedStatus);
        }

        if (bioFlag.HasValue)
        {
            query = query.Where(batch => batch.BioFlag == bioFlag.Value);
        }

        if (!string.IsNullOrWhiteSpace(normalizedSearch))
        {
            query = query.Where(batch =>
                batch.BatchCode.Contains(normalizedSearch) ||
                (batch.Notes != null && batch.Notes.Contains(normalizedSearch)));
        }

        return await query
            .OrderByDescending(batch => batch.CreatedAt)
            .ThenByDescending(batch => batch.Id)
            .ToListAsync(cancellationToken);
    }

    public Task AddAsync(Batch batch, CancellationToken cancellationToken = default)
    {
        NormalizeBatch(batch);
        return _context.Batches.AddAsync(batch, cancellationToken).AsTask();
    }

    public void Update(Batch batch)
    {
        NormalizeBatch(batch);
        _context.Batches.Update(batch);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }

    private static void NormalizeBatch(Batch batch)
    {
        batch.BatchCode = Normalize(batch.BatchCode)!;
        batch.BatchType = Normalize(batch.BatchType)!;
        batch.Status = Normalize(batch.Status)!;
        batch.Notes = Normalize(batch.Notes);
    }

    private static string? Normalize(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
