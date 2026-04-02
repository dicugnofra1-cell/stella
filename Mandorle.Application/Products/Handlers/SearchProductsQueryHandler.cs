using Mandorle.Application.Products.Mapping;
using Mandorle.Application.Products.Models;
using Mandorle.Application.Products.Queries;
using Mandorle.Domain.Interfaces;
using MediatR;

namespace Mandorle.Application.Products.Handlers;

public class SearchProductsQueryHandler : IRequestHandler<SearchProductsQuery, IReadOnlyList<ProductDto>>
{
    private readonly IProductRepository _repository;

    public SearchProductsQueryHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<ProductDto>> Handle(SearchProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await _repository.SearchAsync(
            request.Search,
            request.Category,
            request.Active,
            request.ChannelB2BEnabled,
            request.ChannelB2CEnabled,
            cancellationToken);

        return products.Select(product => product.ToDto()).ToList();
    }
}
