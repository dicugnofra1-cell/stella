using Mandorle.Application.Orders.Commands;
using Mandorle.Domain.Interfaces;
using MediatR;

namespace Mandorle.Application.Orders.Handlers;

public class SetOrderStatusCommandHandler : IRequestHandler<SetOrderStatusCommand, bool>
{
    private readonly IOrderRepository _orderRepository;

    public SetOrderStatusCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<bool> Handle(SetOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.Id, cancellationToken: cancellationToken);
        if (order is null)
        {
            return false;
        }

        order.Status = request.Status;
        order.UpdatedAt = DateTime.UtcNow;

        _orderRepository.Update(order);
        await _orderRepository.SaveChangesAsync(cancellationToken);

        return true;
    }
}
