using Mandorle.Application.Inventory.Models;
using MediatR;

namespace Mandorle.Application.Inventory.Queries;

public record GetInventoryBalanceByProductQuery(int ProductId) : IRequest<InventoryBalanceDto>;
