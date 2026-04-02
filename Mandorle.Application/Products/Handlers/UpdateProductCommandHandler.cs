using Mandorle.Application.Products.Commands;
using Mandorle.Application.Products.Mapping;
using Mandorle.Application.Products.Models;
using Mandorle.Domain.Interfaces;
using MediatR;

namespace Mandorle.Application.Products.Handlers;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, ProductDto?>
{
    private readonly IProductRepository _repository;

    public UpdateProductCommandHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<ProductDto?> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (product is null)
        {
            return null;
        }

        if (await _repository.ExistsBySkuAsync(request.Sku, request.Id, cancellationToken))
        {
            throw new InvalidOperationException("A product with the same SKU already exists.");
        }

        product.Sku = request.Sku;
        product.Name = request.Name;
        product.Description = request.Description;
        product.UnitOfMeasure = request.UnitOfMeasure;
        product.Category = request.Category;
        product.ChannelB2BEnabled = request.ChannelB2BEnabled;
        product.ChannelB2CEnabled = request.ChannelB2CEnabled;
        product.Active = request.Active;
        product.UpdatedAt = DateTime.UtcNow;

        _repository.Update(product);
        await _repository.SaveChangesAsync(cancellationToken);

        return product.ToDto();
    }
}
