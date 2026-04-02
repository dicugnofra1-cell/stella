namespace Mandorle.Application.Customers.Models;

public record CustomerExistenceCheckResultDto(
    bool EmailExists,
    bool PecExists,
    bool SpidIdentifierExists,
    bool VatNumberExists);
