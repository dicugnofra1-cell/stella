using Mandorle.Application.Products.Models;
using MediatR;

namespace Mandorle.Application.Products.Commands;

public record UpdateProductCommand(
    int Id,
    string Sku,
    string Name,
    string? Description,
    string UnitOfMeasure,
    string? Category,
    bool ChannelB2BEnabled,
    bool ChannelB2CEnabled,
    bool Active) : IRequest<ProductDto?>;
