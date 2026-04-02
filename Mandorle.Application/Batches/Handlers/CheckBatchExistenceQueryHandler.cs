using Mandorle.Application.Batches.Models;
using Mandorle.Application.Batches.Queries;
using Mandorle.Domain.Interfaces;
using MediatR;

namespace Mandorle.Application.Batches.Handlers;

public class CheckBatchExistenceQueryHandler : IRequestHandler<CheckBatchExistenceQuery, BatchExistenceCheckResultDto>
{
    private readonly IBatchRepository _batchRepository;

    public CheckBatchExistenceQueryHandler(IBatchRepository batchRepository)
    {
        _batchRepository = batchRepository;
    }

    public async Task<BatchExistenceCheckResultDto> Handle(CheckBatchExistenceQuery request, CancellationToken cancellationToken)
    {
        var exists = await _batchRepository.ExistsByBatchCodeAsync(request.BatchCode, request.ExcludeBatchId, cancellationToken);
        return new BatchExistenceCheckResultDto(exists);
    }
}
