using Mandorle.Application.Products.Models;
using MediatR;

namespace Mandorle.Application.Products.Queries;

public record SearchProductsQuery(
    string? Search,
    string? Category,
    bool? Active,
    bool? ChannelB2BEnabled,
    bool? ChannelB2CEnabled) : IRequest<IReadOnlyList<ProductDto>>;
