using Mandorle.Application.Orders.Mapping;
using Mandorle.Application.Orders.Models;
using Mandorle.Application.Orders.Queries;
using Mandorle.Domain.Interfaces;
using MediatR;

namespace Mandorle.Application.Orders.Handlers;

public class SearchOrdersQueryHandler : IRequestHandler<SearchOrdersQuery, IReadOnlyList<OrderDto>>
{
    private readonly IOrderRepository _orderRepository;

    public SearchOrdersQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<IReadOnlyList<OrderDto>> Handle(SearchOrdersQuery request, CancellationToken cancellationToken)
    {
        var orders = await _orderRepository.SearchAsync(
            request.CustomerId,
            request.OrderType,
            request.Status,
            request.PaymentStatus,
            request.FromDate,
            request.ToDate,
            cancellationToken);

        return orders.Select(order => order.ToDto()).ToList();
    }
}
