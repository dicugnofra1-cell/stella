using Mandorle.Application.Traceability.Mapping;
using Mandorle.Application.Traceability.Models;
using Mandorle.Application.Traceability.Queries;
using Mandorle.Domain.Interfaces;
using MediatR;

namespace Mandorle.Application.Traceability.Handlers;

public class GetPublicTraceViewByBatchCodeQueryHandler : IRequestHandler<GetPublicTraceViewByBatchCodeQuery, PublicTraceViewDto?>
{
    private readonly IPublicTraceViewRepository _publicTraceViewRepository;

    public GetPublicTraceViewByBatchCodeQueryHandler(IPublicTraceViewRepository publicTraceViewRepository)
    {
        _publicTraceViewRepository = publicTraceViewRepository;
    }

    public async Task<PublicTraceViewDto?> Handle(GetPublicTraceViewByBatchCodeQuery request, CancellationToken cancellationToken)
    {
        var traceView = await _publicTraceViewRepository.GetByBatchCodeAsync(request.BatchCode, cancellationToken);
        return traceView?.ToDto();
    }
}
