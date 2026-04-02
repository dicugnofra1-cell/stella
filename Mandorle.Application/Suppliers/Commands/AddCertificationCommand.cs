using Mandorle.Application.Suppliers.Models;
using MediatR;

namespace Mandorle.Application.Suppliers.Commands;

public record AddCertificationCommand(
    int SupplierId,
    string Type,
    string Authority,
    string? Code,
    DateOnly? IssueDate,
    DateOnly ExpiryDate,
    string Status,
    int? DocumentId) : IRequest<CertificationDto?>;
