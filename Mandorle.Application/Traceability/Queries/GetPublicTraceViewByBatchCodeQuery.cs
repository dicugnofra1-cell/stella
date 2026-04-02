using Mandorle.Application.Traceability.Models;
using MediatR;

namespace Mandorle.Application.Traceability.Queries;

public record GetPublicTraceViewByBatchCodeQuery(string BatchCode) : IRequest<PublicTraceViewDto?>;
