using Mandorle.Application.Inventory.Models;
using MediatR;

namespace Mandorle.Application.Inventory.Queries;

public record GetInventoryAvailabilityByProductQuery(int ProductId) : IRequest<InventoryAvailabilityDto>;
