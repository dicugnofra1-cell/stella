using Mandorle.Application.Products.Mapping;
using Mandorle.Application.Products.Models;
using Mandorle.Application.Products.Queries;
using Mandorle.Domain.Interfaces;
using MediatR;

namespace Mandorle.Application.Products.Handlers;

public class GetProductBySkuQueryHandler : IRequestHandler<GetProductBySkuQuery, ProductDto?>
{
    private readonly IProductRepository _repository;

    public GetProductBySkuQueryHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<ProductDto?> Handle(GetProductBySkuQuery request, CancellationToken cancellationToken)
    {
        var product = await _repository.GetBySkuAsync(request.Sku, cancellationToken);
        return product?.ToDto();
    }
}
