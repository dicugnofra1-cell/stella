namespace Mandorle.Application.Audit.Models;

public record AuditLogDto(
    long Id,
    string EntityName,
    string EntityId,
    string Action,
    string? OldValues,
    string? NewValues,
    string ChangedBy,
    DateTime ChangedAt,
    string? CorrelationId);
