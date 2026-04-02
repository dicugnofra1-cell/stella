using Mandorle.Application.Products.Models;
using MediatR;

namespace Mandorle.Application.Products.Queries;

public record GetProductBySkuQuery(string Sku) : IRequest<ProductDto?>;
