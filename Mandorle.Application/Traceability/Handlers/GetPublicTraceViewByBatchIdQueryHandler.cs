using Mandorle.Application.Traceability.Mapping;
using Mandorle.Application.Traceability.Models;
using Mandorle.Application.Traceability.Queries;
using Mandorle.Domain.Interfaces;
using MediatR;

namespace Mandorle.Application.Traceability.Handlers;

public class GetPublicTraceViewByBatchIdQueryHandler : IRequestHandler<GetPublicTraceViewByBatchIdQuery, PublicTraceViewDto?>
{
    private readonly IPublicTraceViewRepository _publicTraceViewRepository;

    public GetPublicTraceViewByBatchIdQueryHandler(IPublicTraceViewRepository publicTraceViewRepository)
    {
        _publicTraceViewRepository = publicTraceViewRepository;
    }

    public async Task<PublicTraceViewDto?> Handle(GetPublicTraceViewByBatchIdQuery request, CancellationToken cancellationToken)
    {
        var traceView = await _publicTraceViewRepository.GetByBatchIdAsync(request.BatchId, cancellationToken);
        return traceView?.ToDto();
    }
}
