using Mandorle.Domain.Entities;

namespace Mandorle.Domain.Interfaces;

public interface IAuditLogRepository
{
    Task<AuditLog?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AuditLog>> SearchAsync(
        string? entityName,
        string? entityId,
        string? action,
        DateTime? fromDate,
        DateTime? toDate,
        CancellationToken cancellationToken = default);

    Task AddAsync(AuditLog auditLog, CancellationToken cancellationToken = default);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
