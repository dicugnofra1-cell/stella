using Mandorle.Application.Traceability.Models;
using Mandorle.Application.Traceability.Queries;
using Mandorle.Domain.Interfaces;
using MediatR;

namespace Mandorle.Application.Traceability.Handlers;

public class GetPublicTraceViewByBatchCodeQueryHandler : IRequestHandler<GetPublicTraceViewByBatchCodeQuery, PublicTraceViewDto?>
{
    private readonly IBatchRepository _batchRepository;
    private readonly IMediator _mediator;

    public GetPublicTraceViewByBatchCodeQueryHandler(IBatchRepository batchRepository, IMediator mediator)
    {
        _batchRepository = batchRepository;
        _mediator = mediator;
    }

    public async Task<PublicTraceViewDto?> Handle(GetPublicTraceViewByBatchCodeQuery request, CancellationToken cancellationToken)
    {
        var batch = await _batchRepository.GetByBatchCodeAsync(request.BatchCode, cancellationToken);
        return batch is null
            ? null
            : await _mediator.Send(new GetPublicTraceViewByBatchIdQuery(batch.Id), cancellationToken);
    }
}
