namespace Mandorle.Application.Customers.Models;

public record CustomerAddressDto(
    int Id,
    int CustomerId,
    string AddressType,
    string RecipientName,
    string Street,
    string? Street2,
    string City,
    string PostalCode,
    string Province,
    string Country,
    bool IsDefault);
