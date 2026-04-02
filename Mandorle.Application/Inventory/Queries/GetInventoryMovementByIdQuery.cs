using Mandorle.Application.Inventory.Models;
using MediatR;

namespace Mandorle.Application.Inventory.Queries;

public record GetInventoryMovementByIdQuery(int Id) : IRequest<InventoryMovementDto?>;
