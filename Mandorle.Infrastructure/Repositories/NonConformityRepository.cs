using Mandorle.Domain.Entities;
using Mandorle.Domain.Interfaces;
using Mandorle.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Mandorle.Infrastructure.Repositories;

public class NonConformityRepository : INonConformityRepository
{
    private readonly StellaFruttaDbContext _context;

    public NonConformityRepository(StellaFruttaDbContext context)
    {
        _context = context;
    }

    public Task<NonConformity?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _context.NonConformities.FirstOrDefaultAsync(nonConformity => nonConformity.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<NonConformity>> SearchAsync(
        int? batchId,
        string? severity,
        string? status,
        CancellationToken cancellationToken = default)
    {
        var query = _context.NonConformities.AsNoTracking().AsQueryable();
        var normalizedSeverity = Normalize(severity);
        var normalizedStatus = Normalize(status);

        if (batchId.HasValue)
        {
            query = query.Where(nonConformity => nonConformity.BatchId == batchId.Value);
        }

        if (!string.IsNullOrWhiteSpace(normalizedSeverity))
        {
            query = query.Where(nonConformity => nonConformity.Severity == normalizedSeverity);
        }

        if (!string.IsNullOrWhiteSpace(normalizedStatus))
        {
            query = query.Where(nonConformity => nonConformity.Status == normalizedStatus);
        }

        return await query
            .OrderByDescending(nonConformity => nonConformity.OpenedAt)
            .ThenByDescending(nonConformity => nonConformity.Id)
            .ToListAsync(cancellationToken);
    }

    public Task AddAsync(NonConformity nonConformity, CancellationToken cancellationToken = default)
    {
        NormalizeNonConformity(nonConformity);
        return _context.NonConformities.AddAsync(nonConformity, cancellationToken).AsTask();
    }

    public void Update(NonConformity nonConformity)
    {
        NormalizeNonConformity(nonConformity);
        _context.NonConformities.Update(nonConformity);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }

    private static void NormalizeNonConformity(NonConformity nonConformity)
    {
        nonConformity.Severity = Normalize(nonConformity.Severity)!;
        nonConformity.Status = Normalize(nonConformity.Status)!;
        nonConformity.Description = Normalize(nonConformity.Description)!;
        nonConformity.CorrectiveAction = Normalize(nonConformity.CorrectiveAction);
        nonConformity.OpenedBy = Normalize(nonConformity.OpenedBy)!;
        nonConformity.ClosedBy = Normalize(nonConformity.ClosedBy);
    }

    private static string? Normalize(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim().ToUpperInvariant();
    }
}
