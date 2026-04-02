using Mandorle.Application.Customers.Models;
using MediatR;

namespace Mandorle.Application.Customers.Queries;

public record GetCustomerByIdQuery(int Id, bool IncludeAddresses = true) : IRequest<CustomerDto?>;
