using Mandorle.Application.Batches.Commands;
using Mandorle.Application.Batches.Mapping;
using Mandorle.Application.Batches.Models;
using Mandorle.Domain.Entities;
using Mandorle.Domain.Interfaces;
using MediatR;

namespace Mandorle.Application.Batches.Handlers;

public class AddBatchLinkCommandHandler : IRequestHandler<AddBatchLinkCommand, BatchLinkDto>
{
    private readonly IBatchRepository _batchRepository;
    private readonly IBatchLinkRepository _batchLinkRepository;

    public AddBatchLinkCommandHandler(IBatchRepository batchRepository, IBatchLinkRepository batchLinkRepository)
    {
        _batchRepository = batchRepository;
        _batchLinkRepository = batchLinkRepository;
    }

    public async Task<BatchLinkDto> Handle(AddBatchLinkCommand request, CancellationToken cancellationToken)
    {
        await EnsureBatchesExistAsync(request.ParentBatchId, request.ChildBatchId, cancellationToken);

        if (request.ParentBatchId == request.ChildBatchId)
        {
            throw new InvalidOperationException("Parent and child batch cannot be the same.");
        }

        if (await _batchLinkRepository.ExistsAsync(request.ParentBatchId, request.ChildBatchId, cancellationToken: cancellationToken))
        {
            throw new InvalidOperationException("The batch link already exists.");
        }

        var batchLink = new BatchLink
        {
            ParentBatchId = request.ParentBatchId,
            ChildBatchId = request.ChildBatchId,
            QuantityUsed = request.QuantityUsed,
            UnitOfMeasure = request.UnitOfMeasure,
            CreatedAt = DateTime.UtcNow
        };

        await _batchLinkRepository.AddAsync(batchLink, cancellationToken);
        await _batchLinkRepository.SaveChangesAsync(cancellationToken);

        return batchLink.ToDto();
    }

    private async Task EnsureBatchesExistAsync(int parentBatchId, int childBatchId, CancellationToken cancellationToken)
    {
        var parentBatch = await _batchRepository.GetByIdAsync(parentBatchId, cancellationToken);
        if (parentBatch is null)
        {
            throw new InvalidOperationException("The parent batch does not exist.");
        }

        var childBatch = await _batchRepository.GetByIdAsync(childBatchId, cancellationToken);
        if (childBatch is null)
        {
            throw new InvalidOperationException("The child batch does not exist.");
        }
    }
}
