using Mandorle.Application.Quality.Mapping;
using Mandorle.Application.Quality.Models;
using Mandorle.Application.Quality.Queries;
using Mandorle.Domain.Interfaces;
using MediatR;

namespace Mandorle.Application.Quality.Handlers;

public class GetQualityChecksByBatchQueryHandler : IRequestHandler<GetQualityChecksByBatchQuery, IReadOnlyList<QualityCheckDto>>
{
    private readonly IQualityCheckRepository _qualityCheckRepository;

    public GetQualityChecksByBatchQueryHandler(IQualityCheckRepository qualityCheckRepository)
    {
        _qualityCheckRepository = qualityCheckRepository;
    }

    public async Task<IReadOnlyList<QualityCheckDto>> Handle(GetQualityChecksByBatchQuery request, CancellationToken cancellationToken)
    {
        var qualityChecks = await _qualityCheckRepository.GetByBatchIdAsync(request.BatchId, cancellationToken);
        return qualityChecks.Select(qualityCheck => qualityCheck.ToDto()).ToList();
    }
}
