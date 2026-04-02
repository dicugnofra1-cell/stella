using Mandorle.Application.Suppliers.Mapping;
using Mandorle.Application.Suppliers.Models;
using Mandorle.Application.Suppliers.Queries;
using Mandorle.Domain.Interfaces;
using MediatR;

namespace Mandorle.Application.Suppliers.Handlers;

public class GetSupplierDocumentsQueryHandler : IRequestHandler<GetSupplierDocumentsQuery, IReadOnlyList<SupplierDocumentDto>>
{
    private readonly ISupplierDocumentRepository _documentRepository;

    public GetSupplierDocumentsQueryHandler(ISupplierDocumentRepository documentRepository)
    {
        _documentRepository = documentRepository;
    }

    public async Task<IReadOnlyList<SupplierDocumentDto>> Handle(GetSupplierDocumentsQuery request, CancellationToken cancellationToken)
    {
        var documents = await _documentRepository.GetBySupplierIdAsync(request.SupplierId, cancellationToken);
        return documents.Select(document => document.ToDto()).ToList();
    }
}
