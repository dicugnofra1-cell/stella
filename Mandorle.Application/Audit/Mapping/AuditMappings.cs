using Mandorle.Application.Audit.Models;
using Mandorle.Domain.Entities;

namespace Mandorle.Application.Audit.Mapping;

internal static class AuditMappings
{
    public static AuditLogDto ToDto(this AuditLog auditLog)
    {
        return new AuditLogDto(
            auditLog.Id,
            auditLog.EntityName,
            auditLog.EntityId,
            auditLog.Action,
            auditLog.OldValues,
            auditLog.NewValues,
            auditLog.ChangedBy,
            auditLog.ChangedAt,
            auditLog.CorrelationId);
    }
}
