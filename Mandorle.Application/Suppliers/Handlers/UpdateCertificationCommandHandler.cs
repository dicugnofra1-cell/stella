using Mandorle.Application.Suppliers.Commands;
using Mandorle.Application.Suppliers.Mapping;
using Mandorle.Application.Suppliers.Models;
using Mandorle.Domain.Interfaces;
using MediatR;

namespace Mandorle.Application.Suppliers.Handlers;

public class UpdateCertificationCommandHandler : IRequestHandler<UpdateCertificationCommand, CertificationDto?>
{
    private readonly ICertificationRepository _certificationRepository;
    private readonly ISupplierDocumentRepository _documentRepository;

    public UpdateCertificationCommandHandler(
        ICertificationRepository certificationRepository,
        ISupplierDocumentRepository documentRepository)
    {
        _certificationRepository = certificationRepository;
        _documentRepository = documentRepository;
    }

    public async Task<CertificationDto?> Handle(UpdateCertificationCommand request, CancellationToken cancellationToken)
    {
        var certification = await _certificationRepository.GetByIdAsync(request.SupplierId, request.CertificationId, cancellationToken);
        if (certification is null)
        {
            return null;
        }

        if (!await IsDocumentValidAsync(request.SupplierId, request.DocumentId, cancellationToken))
        {
            throw new InvalidOperationException("The selected document does not belong to the supplier.");
        }

        certification.Type = request.Type;
        certification.Authority = request.Authority;
        certification.Code = request.Code;
        certification.IssueDate = request.IssueDate;
        certification.ExpiryDate = request.ExpiryDate;
        certification.Status = request.Status;
        certification.DocumentId = request.DocumentId;

        _certificationRepository.Update(certification);
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
