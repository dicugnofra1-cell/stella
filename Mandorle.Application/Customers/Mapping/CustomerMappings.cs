using Mandorle.Application.Customers.Models;
using Mandorle.Domain.Entities;

namespace Mandorle.Application.Customers.Mapping;

internal static class CustomerMappings
{
    public static CustomerDto ToDto(this Customer customer)
    {
        return new CustomerDto(
            customer.Id,
            customer.Type,
            customer.Name,
            customer.VatNumber,
            customer.Email,
            customer.Pec,
            customer.SpidIdentifier,
            customer.Phone,
            customer.Status,
            customer.CustomerAddresses
                .OrderByDescending(address => address.IsDefault)
                .ThenBy(address => address.AddressType)
                .ThenBy(address => address.Id)
                .Select(address => address.ToDto())
                .ToList());
    }

    public static CustomerAddressDto ToDto(this CustomerAddress address)
    {
        return new CustomerAddressDto(
            address.Id,
            address.CustomerId,
            address.AddressType,
            address.RecipientName,
            address.Street,
            address.Street2,
            address.City,
            address.PostalCode,
            address.Province,
            address.Country,
            address.IsDefault);
    }
}
