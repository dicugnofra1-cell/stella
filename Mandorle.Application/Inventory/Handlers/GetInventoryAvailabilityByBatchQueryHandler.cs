using Mandorle.Application.Inventory.Models;
using Mandorle.Application.Inventory.Queries;
using Mandorle.Domain.Interfaces;
using MediatR;

namespace Mandorle.Application.Inventory.Handlers;

public class GetInventoryAvailabilityByBatchQueryHandler : IRequestHandler<GetInventoryAvailabilityByBatchQuery, InventoryAvailabilityDto>
{
    private readonly IInventoryMovementRepository _inventoryMovementRepository;
    private readonly IStockReservationRepository _stockReservationRepository;

    public GetInventoryAvailabilityByBatchQueryHandler(
        IInventoryMovementRepository inventoryMovementRepository,
        IStockReservationRepository stockReservationRepository)
    {
        _inventoryMovementRepository = inventoryMovementRepository;
        _stockReservationRepository = stockReservationRepository;
    }

    public async Task<InventoryAvailabilityDto> Handle(GetInventoryAvailabilityByBatchQuery request, CancellationToken cancellationToken)
    {
        var physicalStock = await _inventoryMovementRepository.GetBalanceByBatchAsync(request.BatchId, cancellationToken);
        var reservedStock = await _stockReservationRepository.GetReservedQuantityByBatchAsync(request.BatchId, cancellationToken);

        return new InventoryAvailabilityDto(
            null,
            request.BatchId,
            physicalStock,
            reservedStock,
            physicalStock - reservedStock);
    }
}
