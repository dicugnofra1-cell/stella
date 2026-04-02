using Mandorle.Application.Products.Models;
using Mandorle.Domain.Entities;

namespace Mandorle.Application.Products.Mapping;

internal static class ProductMappings
{
    public static ProductDto ToDto(this Product product)
    {
        return new ProductDto(
            product.Id,
            product.Sku,
            product.Name,
            product.Description,
            product.UnitOfMeasure,
            product.Category,
            product.ChannelB2BEnabled,
            product.ChannelB2CEnabled,
            product.Active);
    }
}
