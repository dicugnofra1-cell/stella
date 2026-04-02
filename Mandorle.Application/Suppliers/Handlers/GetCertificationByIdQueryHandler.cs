using Mandorle.Application.Suppliers.Mapping;
using Mandorle.Application.Suppliers.Models;
using Mandorle.Application.Suppliers.Queries;
using Mandorle.Domain.Interfaces;
using MediatR;

namespace Mandorle.Application.Suppliers.Handlers;

public class GetCertificationByIdQueryHandler : IRequestHandler<GetCertificationByIdQuery, CertificationDto?>
{
    private readonly ICertificationRepository _certificationRepository;

    public GetCertificationByIdQueryHandler(ICertificationRepository certificationRepository)
    {
        _certificationRepository = certificationRepository;
    }

    public async Task<CertificationDto?> Handle(GetCertificationByIdQuery request, CancellationToken cancellationToken)
    {
        var certification = await _certificationRepository.GetByIdAsync(request.SupplierId, request.CertificationId, cancellationToken);
        return certification?.ToDto();
    }
}
