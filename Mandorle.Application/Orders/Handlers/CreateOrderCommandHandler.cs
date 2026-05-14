using Mandorle.Application.Orders.Commands;
using Mandorle.Application.Orders.Mapping;
using Mandorle.Application.Orders.Models;
using Mandorle.Domain.Entities;
using Mandorle.Domain.Enums;
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
    private readonly IInventoryMovementRepository _inventoryMovementRepository;
    private readonly IStockReservationRepository _stockReservationRepository;

    public CreateOrderCommandHandler(
        ICustomerRepository customerRepository,
        IOrderRepository orderRepository,
        IOrderItemRepository orderItemRepository,
        IProductRepository productRepository,
        IBatchRepository batchRepository,
        IInventoryMovementRepository inventoryMovementRepository,
        IStockReservationRepository stockReservationRepository)
    {
        _customerRepository = customerRepository;
        _orderRepository = orderRepository;
        _orderItemRepository = orderItemRepository;
        _productRepository = productRepository;
        _batchRepository = batchRepository;
        _inventoryMovementRepository = inventoryMovementRepository;
        _stockReservationRepository = stockReservationRepository;
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
            var reservedBatchId = await ResolveReservedBatchIdAsync(item, cancellationToken);
            await EnsureProductAndBatchAreValidAsync(item.ProductId, reservedBatchId, cancellationToken);

            var orderItem = new OrderItem
            {
                OrderId = order.Id,
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                TaxAmount = item.TaxAmount,
                ReservedBatchId = reservedBatchId,
                CreatedAt = DateTime.UtcNow
            };

            await _orderItemRepository.AddAsync(orderItem, cancellationToken);
            await _orderItemRepository.SaveChangesAsync(cancellationToken);

            if (reservedBatchId.HasValue)
            {
                var reservation = new StockReservation
                {
                    OrderId = order.Id,
                    OrderItemId = orderItem.Id,
                    ProductId = item.ProductId,
                    BatchId = reservedBatchId.Value,
                    Quantity = item.Quantity,
                    Status = StockReservationStatus.Active.ToDbValue(),
                    ReservationType = StockReservationType.Order.ToDbValue(),
                    CreatedAt = DateTime.UtcNow,
                    Notes = "Riserva automatica generata in creazione ordine."
                };

                await _stockReservationRepository.AddAsync(reservation, cancellationToken);
                await _stockReservationRepository.SaveChangesAsync(cancellationToken);
            }
        }

        var createdOrder = await _orderRepository.GetByIdAsync(order.Id, includeItems: true, cancellationToken)
            ?? throw new InvalidOperationException("The created order could not be reloaded.");

        return createdOrder.ToDto();
    }

    private async Task<int?> ResolveReservedBatchIdAsync(CreateOrderItemModel item, CancellationToken cancellationToken)
    {
        if (item.ReservedBatchId.HasValue)
        {
            await EnsureBatchHasAvailabilityAsync(item.ReservedBatchId.Value, item.Quantity, cancellationToken);
            return item.ReservedBatchId.Value;
        }

        var candidates = await _batchRepository.GetSaleCandidatesAsync(
            item.ProductId,
            item.BatchType,
            item.BioFlag,
            cancellationToken);

        foreach (var batch in candidates)
        {
            if (!OperationalEnumMappings.TryParseBatchStatus(batch.Status, out var batchStatus) || !batchStatus.IsEligibleForSale())
            {
                continue;
            }

            var physicalStock = await _inventoryMovementRepository.GetBalanceByBatchAsync(batch.Id, cancellationToken);
            var reservedStock = await _stockReservationRepository.GetReservedQuantityByBatchAsync(batch.Id, cancellationToken);
            var availableStock = physicalStock - reservedStock;

            if (availableStock >= item.Quantity)
            {
                return batch.Id;
            }
        }

        throw new InvalidOperationException("Nessun lotto disponibile compatibile con la quantita richiesta.");
    }

    private async Task EnsureBatchHasAvailabilityAsync(int batchId, decimal quantity, CancellationToken cancellationToken)
    {
        var physicalStock = await _inventoryMovementRepository.GetBalanceByBatchAsync(batchId, cancellationToken);
        var reservedStock = await _stockReservationRepository.GetReservedQuantityByBatchAsync(batchId, cancellationToken);
        var availableStock = physicalStock - reservedStock;

        if (availableStock < quantity)
        {
            throw new InvalidOperationException("Il lotto selezionato non ha disponibilita sufficiente.");
        }
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
