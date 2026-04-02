using Mandorle.Application.Suppliers.Mapping;
using Mandorle.Application.Suppliers.Models;
using Mandorle.Application.Suppliers.Queries;
using Mandorle.Domain.Interfaces;
using MediatR;

namespace Mandorle.Application.Suppliers.Handlers;

public class GetSupplierDocumentByIdQueryHandler : IRequestHandler<GetSupplierDocumentByIdQuery, SupplierDocumentDto?>
{
    private readonly ISupplierDocumentRepository _documentRepository;

    public GetSupplierDocumentByIdQueryHandler(ISupplierDocumentRepository documentRepository)
    {
        _documentRepository = documentRepository;
    }

    public async Task<SupplierDocumentDto?> Handle(GetSupplierDocumentByIdQuery request, CancellationToken cancellationToken)
    {
        var document = await _documentRepository.GetByIdAsync(request.SupplierId, request.DocumentId, cancellationToken);
        return document?.ToDto();
    }
}
