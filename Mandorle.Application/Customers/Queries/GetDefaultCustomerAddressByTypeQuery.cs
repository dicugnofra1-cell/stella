using Mandorle.Application.Customers.Models;
using MediatR;

namespace Mandorle.Application.Customers.Queries;

public record GetDefaultCustomerAddressByTypeQuery(int CustomerId, string AddressType) : IRequest<CustomerAddressDto?>;
