using Mandorle.Application.Quality.Mapping;
using Mandorle.Application.Quality.Models;
using Mandorle.Application.Quality.Queries;
using Mandorle.Domain.Interfaces;
using MediatR;

namespace Mandorle.Application.Quality.Handlers;

public class GetQualityCheckByIdQueryHandler : IRequestHandler<GetQualityCheckByIdQuery, QualityCheckDto?>
{
    private readonly IQualityCheckRepository _qualityCheckRepository;

    public GetQualityCheckByIdQueryHandler(IQualityCheckRepository qualityCheckRepository)
    {
        _qualityCheckRepository = qualityCheckRepository;
    }

    public async Task<QualityCheckDto?> Handle(GetQualityCheckByIdQuery request, CancellationToken cancellationToken)
    {
        var qualityCheck = await _qualityCheckRepository.GetByIdAsync(request.Id, cancellationToken);
        return qualityCheck?.ToDto();
    }
}
