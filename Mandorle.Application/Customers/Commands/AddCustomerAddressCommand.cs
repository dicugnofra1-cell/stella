using Mandorle.Application.Customers.Models;
using MediatR;

namespace Mandorle.Application.Customers.Commands;

public record AddCustomerAddressCommand(
    int CustomerId,
    string AddressType,
    string RecipientName,
    string Street,
    string? Street2,
    string City,
    string PostalCode,
    string Province,
    string Country,
    bool IsDefault) : IRequest<CustomerAddressDto?>;
