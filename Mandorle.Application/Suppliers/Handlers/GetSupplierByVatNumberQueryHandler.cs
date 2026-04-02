using Mandorle.Application.Suppliers.Mapping;
using Mandorle.Application.Suppliers.Models;
using Mandorle.Application.Suppliers.Queries;
using Mandorle.Domain.Interfaces;
using MediatR;

namespace Mandorle.Application.Suppliers.Handlers;

public class GetSupplierByVatNumberQueryHandler : IRequestHandler<GetSupplierByVatNumberQuery, SupplierDto?>
{
    private readonly ISupplierRepository _repository;

    public GetSupplierByVatNumberQueryHandler(ISupplierRepository repository)
    {
        _repository = repository;
    }

    public async Task<SupplierDto?> Handle(GetSupplierByVatNumberQuery request, CancellationToken cancellationToken)
    {
        var supplier = await _repository.GetByVatNumberAsync(request.VatNumber, cancellationToken);
        return supplier?.ToDto();
    }
}
