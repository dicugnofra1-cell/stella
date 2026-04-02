using Mandorle.Application.Suppliers.Commands;
using Mandorle.Application.Suppliers.Mapping;
using Mandorle.Application.Suppliers.Models;
using Mandorle.Domain.Entities;
using Mandorle.Domain.Interfaces;
using MediatR;

namespace Mandorle.Application.Suppliers.Handlers;

public class AddSupplierDocumentCommandHandler : IRequestHandler<AddSupplierDocumentCommand, SupplierDocumentDto?>
{
    private readonly ISupplierRepository _supplierRepository;
    private readonly ISupplierDocumentRepository _documentRepository;

    public AddSupplierDocumentCommandHandler(
        ISupplierRepository supplierRepository,
        ISupplierDocumentRepository documentRepository)
    {
        _supplierRepository = supplierRepository;
        _documentRepository = documentRepository;
    }

    public async Task<SupplierDocumentDto?> Handle(AddSupplierDocumentCommand request, CancellationToken cancellationToken)
    {
        var supplier = await _supplierRepository.GetByIdAsync(request.SupplierId, cancellationToken);
        if (supplier is null)
        {
            return null;
        }

        var document = new SupplierDocument
        {
            SupplierId = request.SupplierId,
            DocumentType = request.DocumentType,
            FileName = request.FileName,
            StoragePath = request.StoragePath,
            UploadedBy = request.UploadedBy,
            UploadedAt = DateTime.UtcNow
        };

        await _documentRepository.AddAsync(document, cancellationToken);
        await _documentRepository.SaveChangesAsync(cancellationToken);

        return document.ToDto();
    }
}
