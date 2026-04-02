using Mandorle.Application.Quality.Commands;
using Mandorle.Domain.Interfaces;
using MediatR;

namespace Mandorle.Application.Quality.Handlers;

public class CloseNonConformityCommandHandler : IRequestHandler<CloseNonConformityCommand, bool>
{
    private readonly INonConformityRepository _nonConformityRepository;

    public CloseNonConformityCommandHandler(INonConformityRepository nonConformityRepository)
    {
        _nonConformityRepository = nonConformityRepository;
    }

    public async Task<bool> Handle(CloseNonConformityCommand request, CancellationToken cancellationToken)
    {
        var nonConformity = await _nonConformityRepository.GetByIdAsync(request.Id, cancellationToken);
        if (nonConformity is null)
        {
            return false;
        }

        nonConformity.Status = "CLOSED";
        nonConformity.ClosedBy = request.ClosedBy;
        nonConformity.ClosedAt = request.ClosedAt ?? DateTime.UtcNow;

        if (!string.IsNullOrWhiteSpace(request.CorrectiveAction))
        {
            nonConformity.CorrectiveAction = request.CorrectiveAction;
        }

        _nonConformityRepository.Update(nonConformity);
        await _nonConformityRepository.SaveChangesAsync(cancellationToken);

        return true;
    }
}
