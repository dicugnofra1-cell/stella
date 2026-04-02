using Mandorle.Application.Orders.Models;
using MediatR;

namespace Mandorle.Application.Orders.Queries;

public record GetOrderByIdQuery(int Id) : IRequest<OrderDto?>;
