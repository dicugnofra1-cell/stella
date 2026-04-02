using Mandorle.Application.Customers.Models;
using MediatR;

namespace Mandorle.Application.Customers.Queries;

public record GetCustomerAddressesQuery(int CustomerId) : IRequest<IReadOnlyList<CustomerAddressDto>>;
