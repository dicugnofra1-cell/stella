using Mandorle.Application.Batches.Models;
using Mandorle.Application.Batches.Queries;
using Mandorle.Domain.Interfaces;
using MediatR;

namespace Mandorle.Application.Batches.Handlers;

public class GetBatchParentDetailsQueryHandler : IRequestHandler<GetBatchParentDetailsQuery, IReadOnlyList<BatchParentDetailDto>>
{
    private readonly IBatchLinkRepository _batchLinkRepository;
    private readonly IBatchRepository _batchRepository;

    public GetBatchParentDetailsQueryHandler(IBatchLinkRepository batchLinkRepository, IBatchRepository batchRepository)
    {
        _batchLinkRepository = batchLinkRepository;
        _batchRepository = batchRepository;
    }

    public async Task<IReadOnlyList<BatchParentDetailDto>> Handle(GetBatchParentDetailsQuery request, CancellationToken cancellationToken)
    {
        var links = await _batchLinkRepository.GetByChildBatchIdAsync(request.ChildBatchId, cancellationToken);
        var result = new List<BatchParentDetailDto>(links.Count);

        foreach (var link in links)
        {
            var parentBatch = await _batchRepository.GetByIdAsync(link.ParentBatchId, cancellationToken);
            if (parentBatch is null)
            {
                continue;
            }

            result.Add(new BatchParentDetailDto(
                BatchLinkId: link.Id,
                ParentBatchId: parentBatch.Id,
                ParentBatchCode: parentBatch.BatchCode,
                ProductId: parentBatch.ProductId,
                BatchType: parentBatch.BatchType,
                Variety: parentBatch.Variety,
                BioFlag: parentBatch.BioFlag,
                QuantityUsed: link.QuantityUsed,
                UnitOfMeasure: link.UnitOfMeasure,
                SupplierId: parentBatch.SupplierId));
        }

        return result;
    }
}
