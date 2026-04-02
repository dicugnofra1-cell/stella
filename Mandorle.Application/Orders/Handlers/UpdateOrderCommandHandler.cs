using Mandorle.Application.Orders.Commands;
using Mandorle.Application.Orders.Mapping;
using Mandorle.Application.Orders.Models;
using Mandorle.Domain.Interfaces;
using MediatR;

namespace Mandorle.Application.Orders.Handlers;

public class UpdateOrderCommandHandler : IRequestHandler<UpdateOrderCommand, OrderDto?>
{
    private readonly IOrderRepository _orderRepository;

    public UpdateOrderCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<OrderDto?> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.Id, includeItems: true, cancellationToken);
        if (order is null)
        {
            return null;
        }

        order.OrderType = request.OrderType;
        order.Status = request.Status;
        order.PaymentStatus = request.PaymentStatus;
        order.TotalAmount = request.TotalAmount;
        order.Currency = request.Currency;
        order.Notes = request.Notes;
        order.UpdatedAt = DateTime.UtcNow;

        _orderRepository.Update(order);
        await _orderRepository.SaveChangesAsync(cancellationToken);

        return order.ToDto();
    }
}
