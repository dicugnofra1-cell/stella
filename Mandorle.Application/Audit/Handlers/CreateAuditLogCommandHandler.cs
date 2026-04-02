using Mandorle.Application.Audit.Commands;
using Mandorle.Application.Audit.Mapping;
using Mandorle.Application.Audit.Models;
using Mandorle.Domain.Entities;
using Mandorle.Domain.Interfaces;
using MediatR;

namespace Mandorle.Application.Audit.Handlers;

public class CreateAuditLogCommandHandler : IRequestHandler<CreateAuditLogCommand, AuditLogDto>
{
    private readonly IAuditLogRepository _auditLogRepository;

    public CreateAuditLogCommandHandler(IAuditLogRepository auditLogRepository)
    {
        _auditLogRepository = auditLogRepository;
    }

    public async Task<AuditLogDto> Handle(CreateAuditLogCommand request, CancellationToken cancellationToken)
    {
        var auditLog = new AuditLog
        {
            EntityName = request.EntityName,
            EntityId = request.EntityId,
            Action = request.Action,
            OldValues = request.OldValues,
            NewValues = request.NewValues,
            ChangedBy = request.ChangedBy,
            ChangedAt = request.ChangedAt ?? DateTime.UtcNow,
            CorrelationId = request.CorrelationId
        };

        await _auditLogRepository.AddAsync(auditLog, cancellationToken);
        await _auditLogRepository.SaveChangesAsync(cancellationToken);

        return auditLog.ToDto();
    }
}
