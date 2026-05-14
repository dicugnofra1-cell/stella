using Mandorle.Application.Inventory.Models;
using MediatR;

namespace Mandorle.Application.Inventory.Queries;

public record GetInventoryAvailabilityByBatchQuery(int BatchId) : IRequest<InventoryAvailabilityDto>;
