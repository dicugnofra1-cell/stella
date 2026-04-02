namespace Mandorle.Application.Suppliers.Models;

public record SupplierExistenceCheckResultDto(
    bool EmailExists,
    bool VatNumberExists);
