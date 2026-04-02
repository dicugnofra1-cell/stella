using Mandorle.Application.Batches.Mapping;
using Mandorle.Application.Batches.Models;
using Mandorle.Application.Batches.Queries;
using Mandorle.Domain.Interfaces;
using MediatR;

namespace Mandorle.Application.Batches.Handlers;

public class SearchBatchesQueryHandler : IRequestHandler<SearchBatchesQuery, IReadOnlyList<BatchDto>>
{
    private readonly IBatchRepository _batchRepository;

    public SearchBatchesQueryHandler(IBatchRepository batchRepository)
    {
        _batchRepository = batchRepository;
    }

    public async Task<IReadOnlyList<BatchDto>> Handle(SearchBatchesQuery request, CancellationToken cancellationToken)
    {
        var batches = await _batchRepository.SearchAsync(
            request.Search,
            request.ProductId,
            request.SupplierId,
            request.BatchType,
            request.Status,
            request.BioFlag,
            cancellationToken);

        return batches.Select(batch => batch.ToDto()).ToList();
    }
}
