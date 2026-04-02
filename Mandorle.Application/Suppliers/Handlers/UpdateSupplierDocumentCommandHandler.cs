using Mandorle.Application.Suppliers.Commands;
using Mandorle.Application.Suppliers.Mapping;
using Mandorle.Application.Suppliers.Models;
using Mandorle.Domain.Interfaces;
using MediatR;

namespace Mandorle.Application.Suppliers.Handlers;

public class UpdateSupplierDocumentCommandHandler : IRequestHandler<UpdateSupplierDocumentCommand, SupplierDocumentDto?>
{
    private readonly ISupplierDocumentRepository _documentRepository;

    public UpdateSupplierDocumentCommandHandler(ISupplierDocumentRepository documentRepository)
    {
        _documentRepository = documentRepository;
    }

    public async Task<SupplierDocumentDto?> Handle(UpdateSupplierDocumentCommand request, CancellationToken cancellationToken)
    {
        var document = await _documentRepository.GetByIdAsync(request.SupplierId, request.DocumentId, cancellationToken);
        if (document is null)
        {
            return null;
        }

        document.DocumentType = request.DocumentType;
        document.FileName = request.FileName;
        document.StoragePath = request.StoragePath;
        document.UploadedBy = request.UploadedBy;

        _documentRepository.Update(document);
        await _documentRepository.SaveChangesAsync(cancellationToken);

        return document.ToDto();
    }
}
