namespace Mandorle.Application.Suppliers.Models;

public record SupplierDto(
    int Id,
    string Name,
    string? VatNumber,
    string? Address,
    string? Email,
    string? Phone,
    string Status);
