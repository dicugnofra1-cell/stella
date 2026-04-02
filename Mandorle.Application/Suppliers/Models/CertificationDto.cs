namespace Mandorle.Application.Suppliers.Models;

public record CertificationDto(
    int Id,
    int SupplierId,
    string Type,
    string Authority,
    string? Code,
    DateOnly? IssueDate,
    DateOnly ExpiryDate,
    string Status,
    int? DocumentId);
