using Mandorle.Application.StockReservations.Commands;
using Mandorle.Application.StockReservations.Mapping;
using Mandorle.Application.StockReservations.Models;
using Mandorle.Domain.Entities;
using Mandorle.Domain.Interfaces;
using MediatR;

namespace Mandorle.Application.StockReservations.Handlers;

public class CreateStockReservationCommandHandler : IRequestHandler<CreateStockReservationCommand, StockReservationDto>
{
    private readonly IStockReservationRepository _stockReservationRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderItemRepository _orderItemRepository;
    private readonly IProductRepository _productRepository;
    private readonly IBatchRepository _batchRepository;
    private readonly IInventoryMovementRepository _inventoryMovementRepository;

    public CreateStockReservationCommandHandler(
        IStockReservationRepository stockReservationRepository,
        IOrderRepository orderRepository,
        IOrderItemRepository orderItemRepository,
        IProductRepository productRepository,
        IBatchRepository batchRepository,
        IInventoryMovementRepository inventoryMovementRepository)
    {
        _stockReservationRepository = stockReservationRepository;
        _orderRepository = orderRepository;
        _orderItemRepository = orderItemRepository;
        _productRepository = productRepository;
        _batchRepository = batchRepository;
        _inventoryMovementRepository = inventoryMovementRepository;
    }

    public async Task<StockReservationDto> Handle(CreateStockReservationCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException("The selected order does not exist.");

        var orderItem = await _orderItemRepository.GetByIdAsync(request.OrderItemId, cancellationToken)
            ?? throw new InvalidOperationException("The selected order item does not exist.");

        if (orderItem.OrderId != order.Id)
        {
            throw new InvalidOperationException("The selected order item does not belong to the selected order.");
        }

        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken)
            ?? throw new InvalidOperationException("The selected product does not exist.");

        if (orderItem.ProductId != product.Id)
        {
            throw new InvalidOperationException("The selected order item does not belong to the selected product.");
        }

        if (request.BatchId.HasValue)
        {
            var batch = await _batchRepository.GetByIdAsync(request.BatchId.Value, cancellationToken)
                ?? throw new InvalidOperationException("The selected batch does not exist.");

            if (batch.ProductId != product.Id)
            {
                throw new InvalidOperationException("The selected batch does not belong to the selected product.");
            }
        }

        var availableBalance = request.BatchId.HasValue
            ? await _inventoryMovementRepository.GetBalanceByBatchAsync(request.BatchId.Value, cancellationToken)
            : await _inventoryMovementRepository.GetBalanceByProductAsync(request.ProductId, cancellationToken);

        if (availableBalance < request.Quantity)
        {
            throw new InvalidOperationException("Insufficient available stock for the requested reservation.");
        }

        var reservation = new StockReservation
        {
            OrderId = request.OrderId,
            OrderItemId = request.OrderItemId,
            ProductId = request.ProductId,
            BatchId = request.BatchId,
            Quantity = request.Quantity,
            Status = request.Status,
            ReservationType = request.ReservationType,
            ExpiresAt = request.ExpiresAt,
            Notes = request.Notes,
            CreatedAt = DateTime.UtcNow
        };

        await _stockReservationRepository.AddAsync(reservation, cancellationToken);
        await _stockReservationRepository.SaveChangesAsync(cancellationToken);

        return reservation.ToDto();
    }
}
