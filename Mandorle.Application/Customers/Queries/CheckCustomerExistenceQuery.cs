using Mandorle.Application.Customers.Models;
using MediatR;

namespace Mandorle.Application.Customers.Queries;

public record CheckCustomerExistenceQuery(
    string? Email,
    string? Pec,
    string? SpidIdentifier,
    string? VatNumber,
    int? ExcludeCustomerId) : IRequest<CustomerExistenceCheckResultDto>;
