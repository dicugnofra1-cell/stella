using Mandorle.Application.Batches.Commands;
using Mandorle.Domain.Interfaces;
using MediatR;

namespace Mandorle.Application.Batches.Handlers;

public class SetBatchStatusCommandHandler : IRequestHandler<SetBatchStatusCommand, bool>
{
    private readonly IBatchRepository _batchRepository;

    public SetBatchStatusCommandHandler(IBatchRepository batchRepository)
    {
        _batchRepository = batchRepository;
    }

    public async Task<bool> Handle(SetBatchStatusCommand request, CancellationToken cancellationToken)
    {
        var batch = await _batchRepository.GetByIdAsync(request.Id, cancellationToken);
        if (batch is null)
        {
            return false;
        }

        batch.Status = request.Status;
        batch.UpdatedAt = DateTime.UtcNow;

        _batchRepository.Update(batch);
        await _batchRepository.SaveChangesAsync(cancellationToken);

        return true;
    }
}
