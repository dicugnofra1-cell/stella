using Mandorle.Application.Quality.Commands;
using Mandorle.Application.Quality.Mapping;
using Mandorle.Application.Quality.Models;
using Mandorle.Domain.Entities;
using Mandorle.Domain.Interfaces;
using MediatR;

namespace Mandorle.Application.Quality.Handlers;

public class CreateQualityCheckCommandHandler : IRequestHandler<CreateQualityCheckCommand, QualityCheckDto>
{
    private readonly IQualityCheckRepository _qualityCheckRepository;
    private readonly IBatchRepository _batchRepository;

    public CreateQualityCheckCommandHandler(IQualityCheckRepository qualityCheckRepository, IBatchRepository batchRepository)
    {
        _qualityCheckRepository = qualityCheckRepository;
        _batchRepository = batchRepository;
    }

    public async Task<QualityCheckDto> Handle(CreateQualityCheckCommand request, CancellationToken cancellationToken)
    {
        var batch = await _batchRepository.GetByIdAsync(request.BatchId, cancellationToken)
            ?? throw new InvalidOperationException("The selected batch does not exist.");

        var qualityCheck = new QualityCheck
        {
            BatchId = batch.Id,
            ChecklistVersion = request.ChecklistVersion,
            Result = request.Result,
            Notes = request.Notes,
            CheckedAt = request.CheckedAt ?? DateTime.UtcNow,
            CheckedBy = request.CheckedBy
        };

        await _qualityCheckRepository.AddAsync(qualityCheck, cancellationToken);
        await _qualityCheckRepository.SaveChangesAsync(cancellationToken);

        return qualityCheck.ToDto();
    }
}
