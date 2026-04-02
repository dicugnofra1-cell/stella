using Mandorle.Application.Suppliers.Mapping;
using Mandorle.Application.Suppliers.Models;
using Mandorle.Application.Suppliers.Queries;
using Mandorle.Domain.Interfaces;
using MediatR;

namespace Mandorle.Application.Suppliers.Handlers;

public class GetSupplierByEmailQueryHandler : IRequestHandler<GetSupplierByEmailQuery, SupplierDto?>
{
    private readonly ISupplierRepository _repository;

    public GetSupplierByEmailQueryHandler(ISupplierRepository repository)
    {
        _repository = repository;
    }

    public async Task<SupplierDto?> Handle(GetSupplierByEmailQuery request, CancellationToken cancellationToken)
    {
        var supplier = await _repository.GetByEmailAsync(request.Email, cancellationToken);
        return supplier?.ToDto();
    }
}
