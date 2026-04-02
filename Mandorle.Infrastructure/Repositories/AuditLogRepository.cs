using Mandorle.Domain.Entities;
using Mandorle.Domain.Interfaces;
using Mandorle.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Mandorle.Infrastructure.Repositories;

public class AuditLogRepository : IAuditLogRepository
{
    private readonly StellaFruttaDbContext _context;

    public AuditLogRepository(StellaFruttaDbContext context)
    {
        _context = context;
    }

    public Task<AuditLog?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return _context.AuditLogs.FirstOrDefaultAsync(auditLog => auditLog.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<AuditLog>> SearchAsync(
        string? entityName,
        string? entityId,
        string? action,
        DateTime? fromDate,
        DateTime? toDate,
        CancellationToken cancellationToken = default)
    {
        var query = _context.AuditLogs.AsNoTracking().AsQueryable();
        var normalizedEntityName = Normalize(entityName);
        var normalizedEntityId = Normalize(entityId);
        var normalizedAction = Normalize(action);

        if (!string.IsNullOrWhiteSpace(normalizedEntityName))
        {
            query = query.Where(auditLog => auditLog.EntityName == normalizedEntityName);
        }

        if (!string.IsNullOrWhiteSpace(normalizedEntityId))
        {
            query = query.Where(auditLog => auditLog.EntityId == normalizedEntityId);
        }

        if (!string.IsNullOrWhiteSpace(normalizedAction))
        {
            query = query.Where(auditLog => auditLog.Action == normalizedAction);
        }

        if (fromDate.HasValue)
        {
            query = query.Where(auditLog => auditLog.ChangedAt >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            query = query.Where(auditLog => auditLog.ChangedAt <= toDate.Value);
        }

        return await query
            .OrderByDescending(auditLog => auditLog.ChangedAt)
            .ThenByDescending(auditLog => auditLog.Id)
            .ToListAsync(cancellationToken);
    }

    public Task AddAsync(AuditLog auditLog, CancellationToken cancellationToken = default)
    {
        NormalizeAuditLog(auditLog);
        return _context.AuditLogs.AddAsync(auditLog, cancellationToken).AsTask();
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }

    private static void NormalizeAuditLog(AuditLog auditLog)
    {
        auditLog.EntityName = Normalize(auditLog.EntityName)!;
        auditLog.EntityId = Normalize(auditLog.EntityId)!;
        auditLog.Action = Normalize(auditLog.Action)!;
        auditLog.OldValues = Normalize(auditLog.OldValues);
        auditLog.NewValues = Normalize(auditLog.NewValues);
        auditLog.ChangedBy = Normalize(auditLog.ChangedBy)!;
        auditLog.CorrelationId = Normalize(auditLog.CorrelationId);
    }

    private static string? Normalize(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
