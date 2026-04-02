using Mandorle.Domain.Entities;
using Mandorle.Domain.Interfaces;
using Mandorle.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Mandorle.Infrastructure.Repositories;

public class QualityCheckRepository : IQualityCheckRepository
{
    private readonly StellaFruttaDbContext _context;

    public QualityCheckRepository(StellaFruttaDbContext context)
    {
        _context = context;
    }

    public Task<QualityCheck?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _context.QualityChecks.FirstOrDefaultAsync(qualityCheck => qualityCheck.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<QualityCheck>> GetByBatchIdAsync(int batchId, CancellationToken cancellationToken = default)
    {
        return await _context.QualityChecks
            .AsNoTracking()
            .Where(qualityCheck => qualityCheck.BatchId == batchId)
            .OrderByDescending(qualityCheck => qualityCheck.CheckedAt)
            .ThenByDescending(qualityCheck => qualityCheck.Id)
            .ToListAsync(cancellationToken);
    }

    public Task AddAsync(QualityCheck qualityCheck, CancellationToken cancellationToken = default)
    {
        NormalizeQualityCheck(qualityCheck);
        return _context.QualityChecks.AddAsync(qualityCheck, cancellationToken).AsTask();
    }

    public void Update(QualityCheck qualityCheck)
    {
        NormalizeQualityCheck(qualityCheck);
        _context.QualityChecks.Update(qualityCheck);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }

    private static void NormalizeQualityCheck(QualityCheck qualityCheck)
    {
        qualityCheck.ChecklistVersion = Normalize(qualityCheck.ChecklistVersion)!;
        qualityCheck.Result = Normalize(qualityCheck.Result)!;
        qualityCheck.Notes = Normalize(qualityCheck.Notes);
        qualityCheck.CheckedBy = Normalize(qualityCheck.CheckedBy)!;
    }

    private static string? Normalize(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim().ToUpperInvariant();
    }
}
