using Mandorle.Application.Products.Commands;
using Mandorle.Application.Products.Mapping;
using Mandorle.Application.Products.Models;
using Mandorle.Domain.Entities;
using Mandorle.Domain.Interfaces;
using MediatR;

namespace Mandorle.Application.Products.Handlers;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductDto>
{
    private readonly IProductRepository _repository;

    public CreateProductCommandHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        if (await _repository.ExistsBySkuAsync(request.Sku, cancellationToken: cancellationToken))
        {
            throw new InvalidOperationException("A product with the same SKU already exists.");
        }

        var product = new Product
        {
            Sku = request.Sku,
            Name = request.Name,
            Description = request.Description,
            UnitOfMeasure = request.UnitOfMeasure,
            Category = request.Category,
            ChannelB2BEnabled = request.ChannelB2BEnabled,
            ChannelB2CEnabled = request.ChannelB2CEnabled,
            Active = request.Active,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(product, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return product.ToDto();
    }
}
