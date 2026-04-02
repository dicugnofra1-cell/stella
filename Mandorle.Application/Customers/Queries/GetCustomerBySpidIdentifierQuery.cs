using Mandorle.Application.Customers.Models;
using MediatR;

namespace Mandorle.Application.Customers.Queries;

public record GetCustomerBySpidIdentifierQuery(string SpidIdentifier) : IRequest<CustomerDto?>;
