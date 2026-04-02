using Mandorle.Application.Orders.Commands;
using Mandorle.Application.Orders.Mapping;
using Mandorle.Application.Orders.Models;
using Mandorle.Domain.Entities;
using Mandorle.Domain.Interfaces;
using MediatR;

namespace Mandorle.Application.Orders.Handlers;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, OrderDto>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderItemRepository _orderItemRepository;
    private readonly IProductRepository _productRepository;
    private readonly IBatchRepository _batchRepository;

    public CreateOrderCommandHandler(
        ICustomerRepository customerRepository,
        IOrderRepository orderRepository,
        IOrderItemRepository orderItemRepository,
        IProductRepository productRepository,
        IBatchRepository batchRepository)
    {
        _customerRepository = customerRepository;
        _orderRepository = orderRepository;
        _orderItemRepository = orderItemRepository;
        _productRepository = productRepository;
        _batchRepository = batchRepository;
    }

    public async Task<OrderDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(request.CustomerId, cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException("The selected customer does not exist.");

        var order = new Order
        {
            CustomerId = customer.Id,
            OrderType = request.OrderType,
            Status = request.Status,
            PaymentStatus = request.PaymentStatus,
            TotalAmount = request.TotalAmount,
            Currency = request.Currency,
            Notes = request.Notes,
            CreatedAt = DateTime.UtcNow
        };

        await _orderRepository.AddAsync(order, cancellationToken);
        await _orderRepository.SaveChangesAsync(cancellationToken);

        foreach (var item in request.Items)
        {
            await EnsureProductAndBatchAreValidAsync(item.ProductId, item.ReservedBatchId, cancellationToken);

            var orderItem = new OrderItem
            {
                OrderId = order.Id,
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                TaxAmount = item.TaxAmount,
                ReservedBatchId = item.ReservedBatchId,
                CreatedAt = DateTime.UtcNow
            };

            await _orderItemRepository.AddAsync(orderItem, cancellationToken);
        }

        await _orderItemRepository.SaveChangesAsync(cancellationToken);

        var createdOrder = await _orderRepository.GetByIdAsync(order.Id, includeItems: true, cancellationToken)
            ?? throw new InvalidOperationException("The created order could not be reloaded.");

        return createdOrder.ToDto();
    }

    private async Task EnsureProductAndBatchAreValidAsync(int productId, int? reservedBatchId, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(productId, cancellationToken)
            ?? throw new InvalidOperationException("The selected product does not exist.");

        if (reservedBatchId.HasValue)
        {
            var batch = await _batchRepository.GetByIdAsync(reservedBatchId.Value, cancellationToken)
                ?? throw new InvalidOperationException("The selected reserved batch does not exist.");

            if (batch.ProductId != product.Id)
            {
                throw new InvalidOperationException("The selected reserved batch does not belong to the selected product.");
            }
        }
    }
}
