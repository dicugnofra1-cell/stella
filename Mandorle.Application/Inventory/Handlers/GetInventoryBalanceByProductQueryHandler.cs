using Mandorle.Application.Inventory.Models;
using Mandorle.Application.Inventory.Queries;
using Mandorle.Domain.Interfaces;
using MediatR;

namespace Mandorle.Application.Inventory.Handlers;

public class GetInventoryBalanceByProductQueryHandler : IRequestHandler<GetInventoryBalanceByProductQuery, InventoryBalanceDto>
{
    private readonly IInventoryMovementRepository _inventoryMovementRepository;

    public GetInventoryBalanceByProductQueryHandler(IInventoryMovementRepository inventoryMovementRepository)
    {
        _inventoryMovementRepository = inventoryMovementRepository;
    }

    public async Task<InventoryBalanceDto> Handle(GetInventoryBalanceByProductQuery request, CancellationToken cancellationToken)
    {
        var balance = await _inventoryMovementRepository.GetBalanceByProductAsync(request.ProductId, cancellationToken);
        return new InventoryBalanceDto(request.ProductId, null, balance);
    }
}
