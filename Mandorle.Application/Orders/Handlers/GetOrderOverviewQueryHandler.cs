using Mandorle.Application.Orders.Abstractions;
using Mandorle.Application.Orders.Models;
using Mandorle.Application.Orders.Queries;
using MediatR;

namespace Mandorle.Application.Orders.Handlers;

public class GetOrderOverviewQueryHandler : IRequestHandler<GetOrderOverviewQuery, IReadOnlyList<OrderOverviewRowDto>>
{
    private readonly IOrderOverviewReadRepository _repository;

    public GetOrderOverviewQueryHandler(IOrderOverviewReadRepository repository)
    {
        _repository = repository;
    }

    public Task<IReadOnlyList<OrderOverviewRowDto>> Handle(GetOrderOverviewQuery request, CancellationToken cancellationToken)
    {
        return _repository.SearchAsync(request.OrderType, request.Search, cancellationToken);
    }
}
