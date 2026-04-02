using Mandorle.Application.Inventory.Models;
using MediatR;

namespace Mandorle.Application.Inventory.Queries;

public record GetInventoryBalanceByBatchQuery(int BatchId) : IRequest<InventoryBalanceDto>;
