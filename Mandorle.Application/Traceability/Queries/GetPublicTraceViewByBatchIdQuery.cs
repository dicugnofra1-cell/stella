using Mandorle.Application.Traceability.Models;
using MediatR;

namespace Mandorle.Application.Traceability.Queries;

public record GetPublicTraceViewByBatchIdQuery(int BatchId) : IRequest<PublicTraceViewDto?>;
