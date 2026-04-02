using Mandorle.Application.Inventory.Models;
using Mandorle.Domain.Entities;

namespace Mandorle.Application.Inventory.Mapping;

internal static class InventoryMappings
{
    public static InventoryMovementDto ToDto(this InventoryMovement movement)
    {
        return new InventoryMovementDto(
            movement.Id,
            movement.ProductId,
            movement.BatchId,
            movement.MovementType,
            movement.Quantity,
            movement.MovementDate,
            movement.Reason,
            movement.ReferenceType,
            movement.ReferenceId,
            movement.UserId);
    }
}
