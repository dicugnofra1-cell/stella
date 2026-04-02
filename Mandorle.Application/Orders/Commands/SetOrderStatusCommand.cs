using MediatR;

namespace Mandorle.Application.Orders.Commands;

public record SetOrderStatusCommand(int Id, string Status) : IRequest<bool>;
