using Mandorle.Application.Batches.Mapping;
using Mandorle.Application.Batches.Models;
using Mandorle.Application.Batches.Queries;
using Mandorle.Domain.Interfaces;
using MediatR;

namespace Mandorle.Application.Batches.Handlers;

public class GetBatchChildrenQueryHandler : IRequestHandler<GetBatchChildrenQuery, IReadOnlyList<BatchLinkDto>>
{
    private readonly IBatchLinkRepository _batchLinkRepository;

    public GetBatchChildrenQueryHandler(IBatchLinkRepository batchLinkRepository)
    {
        _batchLinkRepository = batchLinkRepository;
    }

    public async Task<IReadOnlyList<BatchLinkDto>> Handle(GetBatchChildrenQuery request, CancellationToken cancellationToken)
    {
        var batchLinks = await _batchLinkRepository.GetByParentBatchIdAsync(request.ParentBatchId, cancellationToken);
        return batchLinks.Select(batchLink => batchLink.ToDto()).ToList();
    }
}
