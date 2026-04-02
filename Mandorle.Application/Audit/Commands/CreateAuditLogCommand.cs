using Mandorle.Application.Audit.Models;
using MediatR;

namespace Mandorle.Application.Audit.Commands;

public record CreateAuditLogCommand(
    string EntityName,
    string EntityId,
    string Action,
    string? OldValues,
    string? NewValues,
    string ChangedBy,
    DateTime? ChangedAt,
    string? CorrelationId) : IRequest<AuditLogDto>;
