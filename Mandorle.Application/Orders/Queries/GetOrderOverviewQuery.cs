using Mandorle.Application.Orders.Models;
using MediatR;

namespace Mandorle.Application.Orders.Queries;

public record GetOrderOverviewQuery(string? OrderType, string? Search) : IRequest<IReadOnlyList<OrderOverviewRowDto>>;
