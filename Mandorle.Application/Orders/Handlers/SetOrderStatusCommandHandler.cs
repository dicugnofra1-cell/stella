using Mandorle.Application.Orders.Commands;
using Mandorle.Domain.Entities;
using Mandorle.Domain.Enums;
using Mandorle.Domain.Interfaces;
using MediatR;

namespace Mandorle.Application.Orders.Handlers;

public class SetOrderStatusCommandHandler : IRequestHandler<SetOrderStatusCommand, bool>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IStockReservationRepository _stockReservationRepository;
    private readonly IInventoryMovementRepository _inventoryMovementRepository;
    private readonly IBatchRepository _batchRepository;
    private readonly IProductRepository _productRepository;
    private readonly IPublicTraceViewRepository _publicTraceViewRepository;
    private readonly ISupplierRepository _supplierRepository;

    public SetOrderStatusCommandHandler(
        IOrderRepository orderRepository,
        IStockReservationRepository stockReservationRepository,
        IInventoryMovementRepository inventoryMovementRepository,
        IBatchRepository batchRepository,
        IProductRepository productRepository,
        IPublicTraceViewRepository publicTraceViewRepository,
        ISupplierRepository supplierRepository)
    {
        _orderRepository = orderRepository;
        _stockReservationRepository = stockReservationRepository;
        _inventoryMovementRepository = inventoryMovementRepository;
        _batchRepository = batchRepository;
        _productRepository = productRepository;
        _publicTraceViewRepository = publicTraceViewRepository;
        _supplierRepository = supplierRepository;
    }

    public async Task<bool> Handle(SetOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.Id, includeItems: true, cancellationToken: cancellationToken);
        if (order is null)
        {
            return false;
        }

        var normalizedStatus = Normalize(request.Status)!;

        if (OperationalEnumMappings.TryParseOrderStatus(normalizedStatus, out var orderStatus) && orderStatus.IsStockExit())
        {
            await ConsumeOrderReservationsAsync(order, cancellationToken);
        }

        order.Status = normalizedStatus;
        order.UpdatedAt = DateTime.UtcNow;

        _orderRepository.Update(order);
        await _orderRepository.SaveChangesAsync(cancellationToken);

        return true;
    }

    private async Task ConsumeOrderReservationsAsync(Order order, CancellationToken cancellationToken)
    {
        var reservations = await _stockReservationRepository.SearchAsync(
            order.Id,
            null,
            null,
            null,
            null,
            null,
            cancellationToken);

        var activeReservations = reservations
            .Where(reservation => reservation.BatchId.HasValue)
            .Where(reservation => OperationalEnumMappings.TryParseStockReservationStatus(reservation.Status, out var reservationStatus) && reservationStatus.CountsAsReserved())
            .ToList();

        if (activeReservations.Count == 0)
        {
            return;
        }

        foreach (var reservation in activeReservations)
        {
            var batchId = reservation.BatchId!.Value;
            var batch = await _batchRepository.GetByIdAsync(batchId, cancellationToken)
                ?? throw new InvalidOperationException("Il lotto associato alla riserva non esiste.");
            var product = await _productRepository.GetByIdAsync(reservation.ProductId, cancellationToken)
                ?? throw new InvalidOperationException("Il prodotto associato alla riserva non esiste.");
            var currentBalance = await _inventoryMovementRepository.GetBalanceByBatchAsync(batchId, cancellationToken);

            if (currentBalance < reservation.Quantity)
            {
                throw new InvalidOperationException("Disponibilita insufficiente per completare lo scarico del lotto riservato.");
            }

            var movement = new InventoryMovement
            {
                ProductId = reservation.ProductId,
                BatchId = batchId,
                MovementType = InventoryMovementType.Unload.ToDbValue(),
                Quantity = reservation.Quantity,
                MovementDate = DateTime.UtcNow,
                Reason = GetUnloadReason(order.OrderType),
                ReferenceType = MovementReferenceType.Order.ToDbValue(),
                ReferenceId = order.Id.ToString(),
                UserId = "SYSTEM",
                CreatedAt = DateTime.UtcNow
            };

            await _inventoryMovementRepository.AddAsync(movement, cancellationToken);

            reservation.Status = StockReservationStatus.Consumed.ToDbValue();
            reservation.ConsumedAt = DateTime.UtcNow;
            reservation.UpdatedAt = DateTime.UtcNow;
            _stockReservationRepository.Update(reservation);

            await UpsertTraceabilityAsync(batch, product, order, cancellationToken);
        }

        await _inventoryMovementRepository.SaveChangesAsync(cancellationToken);
        await _stockReservationRepository.SaveChangesAsync(cancellationToken);
    }

    private async Task UpsertTraceabilityAsync(Batch batch, Product product, Order order, CancellationToken cancellationToken)
    {
        var traceView = await _publicTraceViewRepository.GetByBatchIdAsync(batch.Id, cancellationToken);
        var supplier = batch.SupplierId.HasValue
            ? await _supplierRepository.GetByIdAsync(batch.SupplierId.Value, cancellationToken)
            : null;

        var originInfo = supplier is null
            ? "Origine merce registrata in Stella."
            : $"Fornitore: {supplier.Name}";

        var mainDates = ComposeMainDates(batch.ProductionDate, order.UpdatedAt ?? DateTime.UtcNow);

        if (traceView is null)
        {
            traceView = new PublicTraceView
            {
                BatchId = batch.Id,
                BatchCode = batch.BatchCode,
                ProductName = product.Name,
                BioFlag = batch.BioFlag,
                OriginInfo = originInfo,
                MainDates = mainDates,
                LastUpdatedAt = DateTime.UtcNow
            };

            await _publicTraceViewRepository.AddAsync(traceView, cancellationToken);
        }
        else
        {
            traceView.BatchCode = batch.BatchCode;
            traceView.ProductName = product.Name;
            traceView.BioFlag = batch.BioFlag;
            traceView.OriginInfo = originInfo;
            traceView.MainDates = mainDates;
            traceView.LastUpdatedAt = DateTime.UtcNow;
            _publicTraceViewRepository.Update(traceView);
        }

        await _publicTraceViewRepository.SaveChangesAsync(cancellationToken);
    }

    private static string GetUnloadReason(string orderType)
    {
        return orderType.Equals("B2B", StringComparison.OrdinalIgnoreCase)
            ? "VENDITA_INGROSSO"
            : "VENDITA_ECOMMERCE";
    }

    private static string ComposeMainDates(DateOnly? productionDate, DateTime orderDate)
    {
        var production = productionDate.HasValue
            ? $"Ingresso lotto: {productionDate.Value:dd/MM/yyyy}"
            : "Ingresso lotto registrato";

        return $"{production} | Vendita: {orderDate:dd/MM/yyyy}";
    }

    private static string? Normalize(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim().ToUpperInvariant();
    }
}
