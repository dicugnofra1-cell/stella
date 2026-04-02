namespace Mandorle.Application.Customers.Models;

public record CustomerDto(
    int Id,
    string Type,
    string Name,
    string? VatNumber,
    string Email,
    string? Pec,
    string? SpidIdentifier,
    string? Phone,
    string Status,
    IReadOnlyList<CustomerAddressDto> Addresses);
