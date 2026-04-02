using Mandorle.Application.Quality.Mapping;
using Mandorle.Application.Quality.Models;
using Mandorle.Application.Quality.Queries;
using Mandorle.Domain.Interfaces;
using MediatR;

namespace Mandorle.Application.Quality.Handlers;

public class GetNonConformityByIdQueryHandler : IRequestHandler<GetNonConformityByIdQuery, NonConformityDto?>
{
    private readonly INonConformityRepository _nonConformityRepository;

    public GetNonConformityByIdQueryHandler(INonConformityRepository nonConformityRepository)
    {
        _nonConformityRepository = nonConformityRepository;
    }

    public async Task<NonConformityDto?> Handle(GetNonConformityByIdQuery request, CancellationToken cancellationToken)
    {
        var nonConformity = await _nonConformityRepository.GetByIdAsync(request.Id, cancellationToken);
        return nonConformity?.ToDto();
    }
}
