using Mandorle.Application.Products.Models;
using MediatR;

namespace Mandorle.Application.Products.Queries;

public record CheckProductExistenceQuery(string Sku, int? ExcludeProductId) : IRequest<ProductExistenceCheckResultDto>;
