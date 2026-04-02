using Mandorle.Application.Customers.Models;
using MediatR;

namespace Mandorle.Application.Customers.Queries;

public record GetCustomerByPecQuery(string Pec) : IRequest<CustomerDto?>;
