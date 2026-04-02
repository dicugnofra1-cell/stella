using Mandorle.Application.Audit.Models;
using MediatR;

namespace Mandorle.Application.Audit.Queries;

public record SearchAuditLogsQuery(
    string? EntityName,
    string? EntityId,
    string? Action,
    DateTime? FromDate,
    DateTime? ToDate) : IRequest<IReadOnlyList<AuditLogDto>>;
