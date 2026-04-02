using Mandorle.Application.Quality.Commands;
using Mandorle.Application.Quality.Mapping;
using Mandorle.Application.Quality.Models;
using Mandorle.Domain.Interfaces;
using MediatR;

namespace Mandorle.Application.Quality.Handlers;

public class UpdateQualityCheckCommandHandler : IRequestHandler<UpdateQualityCheckCommand, QualityCheckDto?>
{
    private readonly IQualityCheckRepository _qualityCheckRepository;

    public UpdateQualityCheckCommandHandler(IQualityCheckRepository qualityCheckRepository)
    {
        _qualityCheckRepository = qualityCheckRepository;
    }

    public async Task<QualityCheckDto?> Handle(UpdateQualityCheckCommand request, CancellationToken cancellationToken)
    {
        var qualityCheck = await _qualityCheckRepository.GetByIdAsync(request.Id, cancellationToken);
        if (qualityCheck is null)
        {
            return null;
        }

        qualityCheck.ChecklistVersion = request.ChecklistVersion;
        qualityCheck.Result = request.Result;
        qualityCheck.Notes = request.Notes;
        qualityCheck.CheckedAt = request.CheckedAt ?? qualityCheck.CheckedAt;
        qualityCheck.CheckedBy = request.CheckedBy;

        _qualityCheckRepository.Update(qualityCheck);
        await _qualityCheckRepository.SaveChangesAsync(cancellationToken);

        return qualityCheck.ToDto();
    }
}
