using MediatR;

namespace Mandorle.Application.Customers.Commands;

public record SetCustomerStatusCommand(int Id, string Status) : IRequest<bool>;
