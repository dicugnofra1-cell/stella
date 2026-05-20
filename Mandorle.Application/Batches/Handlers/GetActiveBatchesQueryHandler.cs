using Mandorle.Application.Batches.Abstractions;
using Mandorle.Application.Batches.Models;
using Mandorle.Application.Batches.Queries;
using MediatR;

namespace Mandorle.Application.Batches.Handlers;

public class GetActiveBatchesQueryHandler : IRequestHandler<GetActiveBatchesQuery, IReadOnlyList<ActiveBatchRowDto>>
{
    private readonly IActiveBatchReadRepository _repository;

    public GetActiveBatchesQueryHandler(IActiveBatchReadRepository repository)
    {
        _repository = repository;
    }

    public Task<IReadOnlyList<ActiveBatchRowDto>> Handle(GetActiveBatchesQuery request, CancellationToken cancellationToken)
    {
        return _repository.SearchActiveAsync(request.Search, cancellationToken);
    }
}
