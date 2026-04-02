namespace Mandorle.Application.Inventory.Models;

public record InventoryMovementDto(
    int Id,
    int ProductId,
    int BatchId,
    string MovementType,
    decimal Quantity,
    DateTime MovementDate,
    string? Reason,
    string? ReferenceType,
    string? ReferenceId,
    string UserId);
