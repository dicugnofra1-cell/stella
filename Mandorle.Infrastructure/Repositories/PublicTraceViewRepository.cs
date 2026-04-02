using Mandorle.Domain.Entities;
using Mandorle.Domain.Interfaces;
using Mandorle.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Mandorle.Infrastructure.Repositories;

public class PublicTraceViewRepository : IPublicTraceViewRepository
{
    private readonly StellaFruttaDbContext _context;

    public PublicTraceViewRepository(StellaFruttaDbContext context)
    {
        _context = context;
    }

    public Task<PublicTraceView?> GetByBatchIdAsync(int batchId, CancellationToken cancellationToken = default)
    {
        return _context.PublicTraceViews.FirstOrDefaultAsync(traceView => traceView.BatchId == batchId, cancellationToken);
    }

    public Task<PublicTraceView?> GetByBatchCodeAsync(string batchCode, CancellationToken cancellationToken = default)
    {
        var normalizedBatchCode = Normalize(batchCode);

        return _context.PublicTraceViews.FirstOrDefaultAsync(
            traceView => traceView.BatchCode == normalizedBatchCode,
            cancellationToken);
    }

    public Task AddAsync(PublicTraceView publicTraceView, CancellationToken cancellationToken = default)
    {
        NormalizeTraceView(publicTraceView);
        return _context.PublicTraceViews.AddAsync(publicTraceView, cancellationToken).AsTask();
    }

    public void Update(PublicTraceView publicTraceView)
    {
        NormalizeTraceView(publicTraceView);
        _context.PublicTraceViews.Update(publicTraceView);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }

    private static void NormalizeTraceView(PublicTraceView publicTraceView)
    {
        publicTraceView.BatchCode = Normalize(publicTraceView.BatchCode)!;
        publicTraceView.ProductName = Normalize(publicTraceView.ProductName)!;
        publicTraceView.OriginInfo = Normalize(publicTraceView.OriginInfo);
        publicTraceView.MainDates = Normalize(publicTraceView.MainDates);
    }

    private static string? Normalize(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
