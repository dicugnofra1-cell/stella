using Mandorle.Application.Orders.Mapping;
using Mandorle.Application.Orders.Models;
using Mandorle.Application.Orders.Queries;
using Mandorle.Domain.Interfaces;
using MediatR;

namespace Mandorle.Application.Orders.Handlers;

public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderDto?>
{
    private readonly IOrderRepository _orderRepository;

    public GetOrderByIdQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<OrderDto?> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.Id, includeItems: true, cancellationToken);
        return order?.ToDto();
    }
}
