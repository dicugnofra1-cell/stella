using Mandorle.Application.Inventory.Models;
using Mandorle.Application.Inventory.Queries;
using Mandorle.Domain.Interfaces;
using MediatR;

namespace Mandorle.Application.Inventory.Handlers;

public class GetInventoryAvailabilityByProductQueryHandler : IRequestHandler<GetInventoryAvailabilityByProductQuery, InventoryAvailabilityDto>
{
    private readonly IInventoryMovementRepository _inventoryMovementRepository;
    private readonly IStockReservationRepository _stockReservationRepository;

    public GetInventoryAvailabilityByProductQueryHandler(
        IInventoryMovementRepository inventoryMovementRepository,
        IStockReservationRepository stockReservationRepository)
    {
        _inventoryMovementRepository = inventoryMovementRepository;
        _stockReservationRepository = stockReservationRepository;
    }

    public async Task<InventoryAvailabilityDto> Handle(GetInventoryAvailabilityByProductQuery request, CancellationToken cancellationToken)
    {
        var physicalStock = await _inventoryMovementRepository.GetBalanceByProductAsync(request.ProductId, cancellationToken);
        var reservedStock = await _stockReservationRepository.GetReservedQuantityByProductAsync(request.ProductId, cancellationToken);

        return new InventoryAvailabilityDto(
            request.ProductId,
            null,
            physicalStock,
            reservedStock,
            physicalStock - reservedStock);
    }
}
