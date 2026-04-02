using Mandorle.Application.Audit.Mapping;
using Mandorle.Application.Audit.Models;
using Mandorle.Application.Audit.Queries;
using Mandorle.Domain.Interfaces;
using MediatR;

namespace Mandorle.Application.Audit.Handlers;

public class GetAuditLogByIdQueryHandler : IRequestHandler<GetAuditLogByIdQuery, AuditLogDto?>
{
    private readonly IAuditLogRepository _auditLogRepository;

    public GetAuditLogByIdQueryHandler(IAuditLogRepository auditLogRepository)
    {
        _auditLogRepository = auditLogRepository;
    }

    public async Task<AuditLogDto?> Handle(GetAuditLogByIdQuery request, CancellationToken cancellationToken)
    {
        var auditLog = await _auditLogRepository.GetByIdAsync(request.Id, cancellationToken);
        return auditLog?.ToDto();
    }
}
