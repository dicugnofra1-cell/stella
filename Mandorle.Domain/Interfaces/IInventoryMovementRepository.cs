using Mandorle.Domain.Entities;

namespace Mandorle.Domain.Interfaces;

public interface IInventoryMovementRepository
{
    Task<InventoryMovement?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<InventoryMovement>> SearchAsync(
        int? productId,
        int? batchId,
        string? movementType,
        DateTime? fromDate,
        DateTime? toDate,
        CancellationToken cancellationToken = default);

    Task<decimal> GetBalanceByBatchAsync(int batchId, CancellationToken cancellationToken = default);

    Task<decimal> GetBalanceByProductAsync(int productId, CancellationToken cancellationToken = default);

    Task AddAsync(InventoryMovement inventoryMovement, CancellationToken cancellationToken = default);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
