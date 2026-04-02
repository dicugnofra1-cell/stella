using Mandorle.Application.Products.Models;
using MediatR;

namespace Mandorle.Application.Products.Queries;

public record GetProductByIdQuery(int Id) : IRequest<ProductDto?>;
