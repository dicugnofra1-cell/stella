using Mandorle.Application.Suppliers.Commands;
using Mandorle.Application.Suppliers.Mapping;
using Mandorle.Application.Suppliers.Models;
using Mandorle.Domain.Entities;
using Mandorle.Domain.Interfaces;
using MediatR;

namespace Mandorle.Application.Suppliers.Handlers;

public class AddCertificationCommandHandler : IRequestHandler<AddCertificationCommand, CertificationDto?>
{
    private readonly ISupplierRepository _supplierRepository;
    private readonly ICertificationRepository _certificationRepository;
    private readonly ISupplierDocumentRepository _documentRepository;

    public AddCertificationCommandHandler(
        ISupplierRepository supplierRepository,
        ICertificationRepository certificationRepository,
        ISupplierDocumentRepository documentRepository)
    {
        _supplierRepository = supplierRepository;
        _certificationRepository = certificationRepository;
        _documentRepository = documentRepository;
    }

    public async Task<CertificationDto?> Handle(AddCertificationCommand request, CancellationToken cancellationToken)
    {
        var supplier = await _supplierRepository.GetByIdAsync(request.SupplierId, cancellationToken);
        if (supplier is null)
        {
            return null;
        }

        if (!await IsDocumentValidAsync(request.SupplierId, request.DocumentId, cancellationToken))
        {
            throw new InvalidOperationException("The selected document does not belong to the supplier.");
        }

        var certification = new Certification
        {
            SupplierId = request.SupplierId,
            Type = request.Type,
            Authority = request.Authority,
            Code = request.Code,
            IssueDate = request.IssueDate,
            ExpiryDate = request.ExpiryDate,
            Status = request.Status,
            DocumentId = request.DocumentId,
            CreatedAt = DateTime.UtcNow
        };

        await _certificationRepository.AddAsync(certification, cancellationToken);
        await _certificationRepository.SaveChangesAsync(cancellationToken);

        return certification.ToDto();
    }

    private async Task<bool> IsDocumentValidAsync(int supplierId, int? documentId, CancellationToken cancellationToken)
    {
        if (!documentId.HasValue)
        {
            return true;
        }

        var document = await _documentRepository.GetByIdAsync(supplierId, documentId.Value, cancellationToken);
        return document is not null;
    }
}
