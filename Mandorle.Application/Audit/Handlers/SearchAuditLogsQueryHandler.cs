using Mandorle.Application.Audit.Mapping;
using Mandorle.Application.Audit.Models;
using Mandorle.Application.Audit.Queries;
using Mandorle.Domain.Interfaces;
using MediatR;

namespace Mandorle.Application.Audit.Handlers;

public class SearchAuditLogsQueryHandler : IRequestHandler<SearchAuditLogsQuery, IReadOnlyList<AuditLogDto>>
{
    private readonly IAuditLogRepository _auditLogRepository;

    public SearchAuditLogsQueryHandler(IAuditLogRepository auditLogRepository)
    {
        _auditLogRepository = auditLogRepository;
    }

    public async Task<IReadOnlyList<AuditLogDto>> Handle(SearchAuditLogsQuery request, CancellationToken cancellationToken)
    {
        var auditLogs = await _auditLogRepository.SearchAsync(
            request.EntityName,
            request.EntityId,
            request.Action,
            request.FromDate,
            request.ToDate,
            cancellationToken);

        return auditLogs.Select(auditLog => auditLog.ToDto()).ToList();
    }
}
