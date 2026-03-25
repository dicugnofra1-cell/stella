using Mandorle.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mandorle.Application.Commands
{
    public record CreateProductCommand(string Name, string SKU) : IRequest<Product>;
}
