using Mandorle.Application.Inventory.Models;
using MediatR;

namespace Mandorle.Application.Inventory.Commands;

public record CreateInventoryMovementCommand(
    int ProductId,
    int BatchId,
    string MovementType,
    decimal Quantity,
    DateTime? MovementDate,
    string? Reason,
    string? ReferenceType,
    string? ReferenceId,
    string UserId) : IRequest<InventoryMovementDto>;
