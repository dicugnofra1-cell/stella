using Mandorle.Application.Inventory.Models;
using Mandorle.Application.Inventory.Queries;
using Mandorle.Domain.Interfaces;
using MediatR;

namespace Mandorle.Application.Inventory.Handlers;

public class GetInventoryBalanceByBatchQueryHandler : IRequestHandler<GetInventoryBalanceByBatchQuery, InventoryBalanceDto>
{
    private readonly IInventoryMovementRepository _inventoryMovementRepository;

    public GetInventoryBalanceByBatchQueryHandler(IInventoryMovementRepository inventoryMovementRepository)
    {
        _inventoryMovementRepository = inventoryMovementRepository;
    }

    public async Task<InventoryBalanceDto> Handle(GetInventoryBalanceByBatchQuery request, CancellationToken cancellationToken)
    {
        var balance = await _inventoryMovementRepository.GetBalanceByBatchAsync(request.BatchId, cancellationToken);
        return new InventoryBalanceDto(null, request.BatchId, balance);
    }
}
