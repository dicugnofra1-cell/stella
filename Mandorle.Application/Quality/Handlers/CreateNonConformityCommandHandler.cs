using Mandorle.Application.Quality.Commands;
using Mandorle.Application.Quality.Mapping;
using Mandorle.Application.Quality.Models;
using Mandorle.Domain.Entities;
using Mandorle.Domain.Interfaces;
using MediatR;

namespace Mandorle.Application.Quality.Handlers;

public class CreateNonConformityCommandHandler : IRequestHandler<CreateNonConformityCommand, NonConformityDto>
{
    private readonly INonConformityRepository _nonConformityRepository;
    private readonly IBatchRepository _batchRepository;

    public CreateNonConformityCommandHandler(INonConformityRepository nonConformityRepository, IBatchRepository batchRepository)
    {
        _nonConformityRepository = nonConformityRepository;
        _batchRepository = batchRepository;
    }

    public async Task<NonConformityDto> Handle(CreateNonConformityCommand request, CancellationToken cancellationToken)
    {
        var batch = await _batchRepository.GetByIdAsync(request.BatchId, cancellationToken)
            ?? throw new InvalidOperationException("The selected batch does not exist.");

        var nonConformity = new NonConformity
        {
            BatchId = batch.Id,
            Severity = request.Severity,
            Status = request.Status,
            Description = request.Description,
            CorrectiveAction = request.CorrectiveAction,
            OpenedBy = request.OpenedBy,
            OpenedAt = request.OpenedAt ?? DateTime.UtcNow
        };

        await _nonConformityRepository.AddAsync(nonConformity, cancellationToken);
        await _nonConformityRepository.SaveChangesAsync(cancellationToken);

        return nonConformity.ToDto();
    }
}
