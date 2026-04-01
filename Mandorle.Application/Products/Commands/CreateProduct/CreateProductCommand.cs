using Mandorle.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mandorle.Application.Products.Commands.CreateProduct
{
    public record CreateProductCommand(string Name, string SKU) : IRequest<int>;
}
