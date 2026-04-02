using Mandorle.Application.Batches.Mapping;
using Mandorle.Application.Batches.Models;
using Mandorle.Application.Batches.Queries;
using Mandorle.Domain.Interfaces;
using MediatR;

namespace Mandorle.Application.Batches.Handlers;

public class GetBatchByCodeQueryHandler : IRequestHandler<GetBatchByCodeQuery, BatchDto?>
{
    private readonly IBatchRepository _batchRepository;

    public GetBatchByCodeQueryHandler(IBatchRepository batchRepository)
    {
        _batchRepository = batchRepository;
    }

    public async Task<BatchDto?> Handle(GetBatchByCodeQuery request, CancellationToken cancellationToken)
    {
        var batch = await _batchRepository.GetByBatchCodeAsync(request.BatchCode, cancellationToken);
        return batch?.ToDto();
    }
}
