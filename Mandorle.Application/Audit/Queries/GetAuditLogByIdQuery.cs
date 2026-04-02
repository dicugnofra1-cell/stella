using Mandorle.Application.Audit.Models;
using MediatR;

namespace Mandorle.Application.Audit.Queries;

public record GetAuditLogByIdQuery(long Id) : IRequest<AuditLogDto?>;
