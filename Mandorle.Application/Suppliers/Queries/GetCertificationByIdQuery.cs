using Mandorle.Application.Suppliers.Models;
using MediatR;

namespace Mandorle.Application.Suppliers.Queries;

public record GetCertificationByIdQuery(int SupplierId, int CertificationId) : IRequest<CertificationDto?>;
