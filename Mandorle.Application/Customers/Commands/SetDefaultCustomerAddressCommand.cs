using MediatR;

namespace Mandorle.Application.Customers.Commands;

public record SetDefaultCustomerAddressCommand(int CustomerId, int AddressId) : IRequest<bool>;
