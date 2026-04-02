using Mandorle.Application.Suppliers.Mapping;
using Mandorle.Application.Suppliers.Models;
using Mandorle.Application.Suppliers.Queries;
using Mandorle.Domain.Interfaces;
using MediatR;

namespace Mandorle.Application.Suppliers.Handlers;

public class SearchSuppliersQueryHandler : IRequestHandler<SearchSuppliersQuery, IReadOnlyList<SupplierDto>>
{
    private readonly ISupplierRepository _repository;

    public SearchSuppliersQueryHandler(ISupplierRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<SupplierDto>> Handle(SearchSuppliersQuery request, CancellationToken cancellationToken)
    {
        var suppliers = await _repository.SearchAsync(request.Search, request.Status, cancellationToken);
        return suppliers.Select(supplier => supplier.ToDto()).ToList();
    }
}
