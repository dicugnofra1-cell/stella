using Mandorle.Application.Quality.Commands;
using Mandorle.Application.Quality.Mapping;
using Mandorle.Application.Quality.Models;
using Mandorle.Domain.Interfaces;
using MediatR;

namespace Mandorle.Application.Quality.Handlers;

public class UpdateNonConformityCommandHandler : IRequestHandler<UpdateNonConformityCommand, NonConformityDto?>
{
    private readonly INonConformityRepository _nonConformityRepository;

    public UpdateNonConformityCommandHandler(INonConformityRepository nonConformityRepository)
    {
        _nonConformityRepository = nonConformityRepository;
    }

    public async Task<NonConformityDto?> Handle(UpdateNonConformityCommand request, CancellationToken cancellationToken)
    {
        var nonConformity = await _nonConformityRepository.GetByIdAsync(request.Id, cancellationToken);
        if (nonConformity is null)
        {
            return null;
        }

        nonConformity.Severity = request.Severity;
        nonConformity.Status = request.Status;
        nonConformity.Description = request.Description;
        nonConformity.CorrectiveAction = request.CorrectiveAction;

        _nonConformityRepository.Update(nonConformity);
        await _nonConformityRepository.SaveChangesAsync(cancellationToken);

        return nonConformity.ToDto();
    }
}
