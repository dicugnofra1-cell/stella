using Mandorle.Application.Products.Models;
using Mandorle.Application.Products.Queries;
using Mandorle.Domain.Interfaces;
using MediatR;

namespace Mandorle.Application.Products.Handlers;

public class CheckProductExistenceQueryHandler : IRequestHandler<CheckProductExistenceQuery, ProductExistenceCheckResultDto>
{
    private readonly IProductRepository _repository;

    public CheckProductExistenceQueryHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<ProductExistenceCheckResultDto> Handle(CheckProductExistenceQuery request, CancellationToken cancellationToken)
    {
        var skuExists = await _repository.ExistsBySkuAsync(request.Sku, request.ExcludeProductId, cancellationToken);
        return new ProductExistenceCheckResultDto(skuExists);
    }
}
