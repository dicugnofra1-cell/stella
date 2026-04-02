using Mandorle.Application.Inventory.Mapping;
using Mandorle.Application.Inventory.Models;
using Mandorle.Application.Inventory.Queries;
using Mandorle.Domain.Interfaces;
using MediatR;

namespace Mandorle.Application.Inventory.Handlers;

public class GetInventoryMovementByIdQueryHandler : IRequestHandler<GetInventoryMovementByIdQuery, InventoryMovementDto?>
{
    private readonly IInventoryMovementRepository _inventoryMovementRepository;

    public GetInventoryMovementByIdQueryHandler(IInventoryMovementRepository inventoryMovementRepository)
    {
        _inventoryMovementRepository = inventoryMovementRepository;
    }

    public async Task<InventoryMovementDto?> Handle(GetInventoryMovementByIdQuery request, CancellationToken cancellationToken)
    {
        var movement = await _inventoryMovementRepository.GetByIdAsync(request.Id, cancellationToken);
        return movement?.ToDto();
    }
}
