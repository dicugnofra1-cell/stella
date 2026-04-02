using Mandorle.Application.Inventory.Commands;
using Mandorle.Application.Inventory.Mapping;
using Mandorle.Application.Inventory.Models;
using Mandorle.Domain.Entities;
using Mandorle.Domain.Interfaces;
using MediatR;

namespace Mandorle.Application.Inventory.Handlers;

public class CreateInventoryMovementCommandHandler : IRequestHandler<CreateInventoryMovementCommand, InventoryMovementDto>
{
    private static readonly string[] NegativeMovementTypes =
    [
        "UNLOAD",
        "RETURN_OUT",
        "WASTE",
        "TRANSFER_OUT",
        "ADJUSTMENT_OUT"
    ];

    private readonly IInventoryMovementRepository _inventoryMovementRepository;
    private readonly IBatchRepository _batchRepository;
    private readonly IProductRepository _productRepository;

    public CreateInventoryMovementCommandHandler(
        IInventoryMovementRepository inventoryMovementRepository,
        IBatchRepository batchRepository,
        IProductRepository productRepository)
    {
        _inventoryMovementRepository = inventoryMovementRepository;
        _batchRepository = batchRepository;
        _productRepository = productRepository;
    }

    public async Task<InventoryMovementDto> Handle(CreateInventoryMovementCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);
        if (product is null)
        {
            throw new InvalidOperationException("The selected product does not exist.");
        }

        var batch = await _batchRepository.GetByIdAsync(request.BatchId, cancellationToken);
        if (batch is null)
        {
            throw new InvalidOperationException("The selected batch does not exist.");
        }

        if (batch.ProductId != request.ProductId)
        {
            throw new InvalidOperationException("The selected batch does not belong to the selected product.");
        }

        var normalizedMovementType = Normalize(request.MovementType)!;

        if (NegativeMovementTypes.Contains(normalizedMovementType))
        {
            var currentBalance = await _inventoryMovementRepository.GetBalanceByBatchAsync(request.BatchId, cancellationToken);
            if (currentBalance < request.Quantity)
            {
                throw new InvalidOperationException("Insufficient batch stock for the requested movement.");
            }
        }

        var movement = new InventoryMovement
        {
            ProductId = request.ProductId,
            BatchId = request.BatchId,
            MovementType = request.MovementType,
            Quantity = request.Quantity,
            MovementDate = request.MovementDate ?? DateTime.UtcNow,
            Reason = request.Reason,
            ReferenceType = request.ReferenceType,
            ReferenceId = request.ReferenceId,
            UserId = request.UserId,
            CreatedAt = DateTime.UtcNow
        };

        await _inventoryMovementRepository.AddAsync(movement, cancellationToken);
        await _inventoryMovementRepository.SaveChangesAsync(cancellationToken);

        return movement.ToDto();
    }

    private static string? Normalize(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim().ToUpperInvariant();
    }
}
