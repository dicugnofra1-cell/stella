using Mandorle.Application.Quality.Mapping;
using Mandorle.Application.Quality.Models;
using Mandorle.Application.Quality.Queries;
using Mandorle.Domain.Interfaces;
using MediatR;

namespace Mandorle.Application.Quality.Handlers;

public class SearchNonConformitiesQueryHandler : IRequestHandler<SearchNonConformitiesQuery, IReadOnlyList<NonConformityDto>>
{
    private readonly INonConformityRepository _nonConformityRepository;

    public SearchNonConformitiesQueryHandler(INonConformityRepository nonConformityRepository)
    {
        _nonConformityRepository = nonConformityRepository;
    }

    public async Task<IReadOnlyList<NonConformityDto>> Handle(SearchNonConformitiesQuery request, CancellationToken cancellationToken)
    {
        var nonConformities = await _nonConformityRepository.SearchAsync(
            request.BatchId,
            request.Severity,
            request.Status,
            cancellationToken);

        return nonConformities.Select(nonConformity => nonConformity.ToDto()).ToList();
    }
}
