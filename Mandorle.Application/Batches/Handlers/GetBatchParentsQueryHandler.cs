using Mandorle.Application.Batches.Mapping;
using Mandorle.Application.Batches.Models;
using Mandorle.Application.Batches.Queries;
using Mandorle.Domain.Interfaces;
using MediatR;

namespace Mandorle.Application.Batches.Handlers;

public class GetBatchParentsQueryHandler : IRequestHandler<GetBatchParentsQuery, IReadOnlyList<BatchLinkDto>>
{
    private readonly IBatchLinkRepository _batchLinkRepository;

    public GetBatchParentsQueryHandler(IBatchLinkRepository batchLinkRepository)
    {
        _batchLinkRepository = batchLinkRepository;
    }

    public async Task<IReadOnlyList<BatchLinkDto>> Handle(GetBatchParentsQuery request, CancellationToken cancellationToken)
    {
        var batchLinks = await _batchLinkRepository.GetByChildBatchIdAsync(request.ChildBatchId, cancellationToken);
        return batchLinks.Select(batchLink => batchLink.ToDto()).ToList();
    }
}
