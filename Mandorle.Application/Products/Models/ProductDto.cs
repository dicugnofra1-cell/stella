namespace Mandorle.Application.Products.Models;

public record ProductDto(
    int Id,
    string Sku,
    string Name,
    string? Description,
    string UnitOfMeasure,
    string? Category,
    bool ChannelB2BEnabled,
    bool ChannelB2CEnabled,
    bool Active);
