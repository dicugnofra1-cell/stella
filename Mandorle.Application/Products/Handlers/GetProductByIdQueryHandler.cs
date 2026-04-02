using Mandorle.Application.Products.Mapping;
using Mandorle.Application.Products.Models;
using Mandorle.Application.Products.Queries;
using Mandorle.Domain.Interfaces;
using MediatR;

namespace Mandorle.Application.Products.Handlers;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto?>
{
    private readonly IProductRepository _repository;

    public GetProductByIdQueryHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<ProductDto?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _repository.GetByIdAsync(request.Id, cancellationToken);
        return product?.ToDto();
    }
}
