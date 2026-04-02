using Mandorle.Application.Batches.Mapping;
using Mandorle.Application.Batches.Models;
using Mandorle.Application.Batches.Queries;
using Mandorle.Domain.Interfaces;
using MediatR;

namespace Mandorle.Application.Batches.Handlers;

public class GetBatchLinkByIdQueryHandler : IRequestHandler<GetBatchLinkByIdQuery, BatchLinkDto?>
{
    private readonly IBatchLinkRepository _batchLinkRepository;

    public GetBatchLinkByIdQueryHandler(IBatchLinkRepository batchLinkRepository)
    {
        _batchLinkRepository = batchLinkRepository;
    }

    public async Task<BatchLinkDto?> Handle(GetBatchLinkByIdQuery request, CancellationToken cancellationToken)
    {
        var batchLink = await _batchLinkRepository.GetByIdAsync(request.ChildBatchId, request.BatchLinkId, cancellationToken);
        return batchLink?.ToDto();
    }
}
