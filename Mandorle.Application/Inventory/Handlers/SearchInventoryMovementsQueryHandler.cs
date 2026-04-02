using Mandorle.Application.Inventory.Mapping;
using Mandorle.Application.Inventory.Models;
using Mandorle.Application.Inventory.Queries;
using Mandorle.Domain.Interfaces;
using MediatR;

namespace Mandorle.Application.Inventory.Handlers;

public class SearchInventoryMovementsQueryHandler : IRequestHandler<SearchInventoryMovementsQuery, IReadOnlyList<InventoryMovementDto>>
{
    private readonly IInventoryMovementRepository _inventoryMovementRepository;

    public SearchInventoryMovementsQueryHandler(IInventoryMovementRepository inventoryMovementRepository)
    {
        _inventoryMovementRepository = inventoryMovementRepository;
    }

    public async Task<IReadOnlyList<InventoryMovementDto>> Handle(SearchInventoryMovementsQuery request, CancellationToken cancellationToken)
    {
        var movements = await _inventoryMovementRepository.SearchAsync(
            request.ProductId,
            request.BatchId,
            request.MovementType,
            request.FromDate,
            request.ToDate,
            cancellationToken);

        return movements.Select(movement => movement.ToDto()).ToList();
    }
}
