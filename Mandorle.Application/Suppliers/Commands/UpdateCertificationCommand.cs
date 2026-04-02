using Mandorle.Application.Suppliers.Models;
using MediatR;

namespace Mandorle.Application.Suppliers.Commands;

public record UpdateCertificationCommand(
    int SupplierId,
    int CertificationId,
    string Type,
    string Authority,
    string? Code,
    DateOnly? IssueDate,
    DateOnly ExpiryDate,
    string Status,
    int? DocumentId) : IRequest<CertificationDto?>;
