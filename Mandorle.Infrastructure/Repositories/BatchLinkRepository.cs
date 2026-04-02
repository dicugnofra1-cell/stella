using Mandorle.Domain.Entities;
using Mandorle.Domain.Interfaces;
using Mandorle.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Mandorle.Infrastructure.Repositories;

public class BatchLinkRepository : IBatchLinkRepository
{
    private readonly StellaFruttaDbContext _context;

    public BatchLinkRepository(StellaFruttaDbContext context)
    {
        _context = context;
    }

    public Task<BatchLink?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _context.BatchLinks.FirstOrDefaultAsync(batchLink => batchLink.Id == id, cancellationToken);
    }

    public Task<BatchLink?> GetByIdAsync(int childBatchId, int id, CancellationToken cancellationToken = default)
    {
        return _context.BatchLinks.FirstOrDefaultAsync(
            batchLink => batchLink.ChildBatchId == childBatchId && batchLink.Id == id,
            cancellationToken);
    }

    public Task<bool> ExistsAsync(int parentBatchId, int childBatchId, int? excludeBatchLinkId = null, CancellationToken cancellationToken = default)
    {
        return _context.BatchLinks.AnyAsync(
            batchLink =>
                batchLink.ParentBatchId == parentBatchId &&
                batchLink.ChildBatchId == childBatchId &&
                (!excludeBatchLinkId.HasValue || batchLink.Id != excludeBatchLinkId.Value),
            cancellationToken);
    }

    public async Task<IReadOnlyList<BatchLink>> GetByChildBatchIdAsync(int childBatchId, CancellationToken cancellationToken = default)
    {
        return await _context.BatchLinks
            .AsNoTracking()
            .Where(batchLink => batchLink.ChildBatchId == childBatchId)
            .OrderBy(batchLink => batchLink.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<BatchLink>> GetByParentBatchIdAsync(int parentBatchId, CancellationToken cancellationToken = default)
    {
        return await _context.BatchLinks
            .AsNoTracking()
            .Where(batchLink => batchLink.ParentBatchId == parentBatchId)
            .OrderBy(batchLink => batchLink.Id)
            .ToListAsync(cancellationToken);
    }

    public Task AddAsync(BatchLink batchLink, CancellationToken cancellationToken = default)
    {
        NormalizeBatchLink(batchLink);
        return _context.BatchLinks.AddAsync(batchLink, cancellationToken).AsTask();
    }

    public void Update(BatchLink batchLink)
    {
        NormalizeBatchLink(batchLink);
        _context.BatchLinks.Update(batchLink);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }

    private static void NormalizeBatchLink(BatchLink batchLink)
    {
        batchLink.UnitOfMeasure = Normalize(batchLink.UnitOfMeasure)!;
    }

    private static string? Normalize(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
