using Mandorle.Application.Suppliers.Models;
using MediatR;

namespace Mandorle.Application.Suppliers.Queries;

public record GetSupplierCertificationsQuery(int SupplierId, string? Type, string? Status) : IRequest<IReadOnlyList<CertificationDto>>;
