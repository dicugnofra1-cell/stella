using Mandorle.Application.Batches.Mapping;
using Mandorle.Application.Batches.Models;
using Mandorle.Application.Batches.Queries;
using Mandorle.Domain.Interfaces;
using MediatR;

namespace Mandorle.Application.Batches.Handlers;

public class GetBatchByIdQueryHandler : IRequestHandler<GetBatchByIdQuery, BatchDto?>
{
    private readonly IBatchRepository _batchRepository;

    public GetBatchByIdQueryHandler(IBatchRepository batchRepository)
    {
        _batchRepository = batchRepository;
    }

    public async Task<BatchDto?> Handle(GetBatchByIdQuery request, CancellationToken cancellationToken)
    {
        var batch = await _batchRepository.GetByIdAsync(request.Id, cancellationToken);
        return batch?.ToDto();
    }
}
