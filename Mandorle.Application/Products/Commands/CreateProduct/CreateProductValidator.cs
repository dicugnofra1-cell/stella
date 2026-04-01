using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mandorle.Application.Products.Commands.CreateProduct
{
    public class CreateProductValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.SKU).NotEmpty();
        }
    }
}
