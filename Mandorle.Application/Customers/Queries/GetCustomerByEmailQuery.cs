using Mandorle.Application.Customers.Models;
using MediatR;

namespace Mandorle.Application.Customers.Queries;

public record GetCustomerByEmailQuery(string Email) : IRequest<CustomerDto?>;
