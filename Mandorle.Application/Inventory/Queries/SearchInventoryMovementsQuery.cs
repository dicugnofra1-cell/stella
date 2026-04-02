using Mandorle.Application.Inventory.Models;
using MediatR;

namespace Mandorle.Application.Inventory.Queries;

public record SearchInventoryMovementsQuery(
    int? ProductId,
    int? BatchId,
    string? MovementType,
    DateTime? FromDate,
    DateTime? ToDate) : IRequest<IReadOnlyList<InventoryMovementDto>>;
