using Mandorle.Application.Suppliers.Mapping;
using Mandorle.Application.Suppliers.Models;
using Mandorle.Application.Suppliers.Queries;
using Mandorle.Domain.Interfaces;
using MediatR;

namespace Mandorle.Application.Suppliers.Handlers;

public class GetSupplierCertificationsQueryHandler : IRequestHandler<GetSupplierCertificationsQuery, IReadOnlyList<CertificationDto>>
{
    private readonly ICertificationRepository _certificationRepository;

    public GetSupplierCertificationsQueryHandler(ICertificationRepository certificationRepository)
    {
        _certificationRepository = certificationRepository;
    }

    public async Task<IReadOnlyList<CertificationDto>> Handle(GetSupplierCertificationsQuery request, CancellationToken cancellationToken)
    {
        var certifications = await _certificationRepository.GetBySupplierIdAsync(
            request.SupplierId,
            request.Type,
            request.Status,
            cancellationToken);

        return certifications.Select(certification => certification.ToDto()).ToList();
    }
}
