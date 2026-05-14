using Mandorle.Domain.Entities;
using Mandorle.Domain.Enums;
using Mandorle.Domain.Interfaces;
using Mandorle.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Mandorle.Infrastructure.Repositories;

public class InventoryMovementRepository : IInventoryMovementRepository
{
    private readonly StellaFruttaDbContext _context;

    public InventoryMovementRepository(StellaFruttaDbContext context)
    {
        _context = context;
    }

    public Task<InventoryMovement?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _context.InventoryMovements.FirstOrDefaultAsync(movement => movement.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<InventoryMovement>> SearchAsync(
        int? productId,
        int? batchId,
        string? movementType,
        DateTime? fromDate,
        DateTime? toDate,
        CancellationToken cancellationToken = default)
    {
        var query = _context.InventoryMovements.AsNoTracking().AsQueryable();
        var normalizedMovementType = Normalize(movementType);

        if (productId.HasValue)
        {
            query = query.Where(movement => movement.ProductId == productId.Value);
        }

        if (batchId.HasValue)
        {
            query = query.Where(movement => movement.BatchId == batchId.Value);
        }

        if (!string.IsNullOrWhiteSpace(normalizedMovementType))
        {
            query = query.Where(movement => movement.MovementType == normalizedMovementType);
        }

        if (fromDate.HasValue)
        {
            query = query.Where(movement => movement.MovementDate >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            query = query.Where(movement => movement.MovementDate <= toDate.Value);
        }

        return await query
            .OrderByDescending(movement => movement.MovementDate)
            .ThenByDescending(movement => movement.Id)
            .ToListAsync(cancellationToken);
    }

    public Task<decimal> GetBalanceByBatchAsync(int batchId, CancellationToken cancellationToken = default)
    {
        return CalculateBalanceAsync(
            _context.InventoryMovements.Where(movement => movement.BatchId == batchId),
            cancellationToken);
    }

    public Task<decimal> GetBalanceByProductAsync(int productId, CancellationToken cancellationToken = default)
    {
        return CalculateBalanceAsync(
            _context.InventoryMovements.Where(movement => movement.ProductId == productId),
            cancellationToken);
    }

    public Task AddAsync(InventoryMovement inventoryMovement, CancellationToken cancellationToken = default)
    {
        NormalizeMovement(inventoryMovement);
        return _context.InventoryMovements.AddAsync(inventoryMovement, cancellationToken).AsTask();
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }

    private async Task<decimal> CalculateBalanceAsync(IQueryable<InventoryMovement> query, CancellationToken cancellationToken)
    {
        var movements = await query
            .AsNoTracking()
            .Select(movement => new { movement.MovementType, movement.Quantity })
            .ToListAsync(cancellationToken);

        return movements.Sum(movement => SignedQuantity(movement.MovementType, movement.Quantity));
    }

    private static decimal SignedQuantity(string movementType, decimal quantity)
    {
        if (!OperationalEnumMappings.TryParseInventoryMovementType(movementType, out var parsedMovementType))
        {
            return 0m;
        }

        if (parsedMovementType.IsNegative())
        {
            return -quantity;
        }

        return quantity;
    }

    private static void NormalizeMovement(InventoryMovement inventoryMovement)
    {
        inventoryMovement.MovementType = Normalize(inventoryMovement.MovementType)!;
        inventoryMovement.Reason = Normalize(inventoryMovement.Reason);
        inventoryMovement.ReferenceType = Normalize(inventoryMovement.ReferenceType);
        inventoryMovement.ReferenceId = Normalize(inventoryMovement.ReferenceId);
        inventoryMovement.UserId = Normalize(inventoryMovement.UserId)!;
    }

    private static string? Normalize(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim().ToUpperInvariant();
    }
}
