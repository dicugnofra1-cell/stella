using Mandorle.Application.Customers.Models;
using MediatR;

namespace Mandorle.Application.Customers.Queries;

public record GetCustomerByVatNumberQuery(string VatNumber) : IRequest<CustomerDto?>;
