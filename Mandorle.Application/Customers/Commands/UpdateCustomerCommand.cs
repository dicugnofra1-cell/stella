using Mandorle.Application.Customers.Models;
using MediatR;

namespace Mandorle.Application.Customers.Commands;

public record UpdateCustomerCommand(
    int Id,
    string Type,
    string Name,
    string? VatNumber,
    string Email,
    string? Pec,
    string? SpidIdentifier,
    string? Phone,
    string Status) : IRequest<CustomerDto?>;
