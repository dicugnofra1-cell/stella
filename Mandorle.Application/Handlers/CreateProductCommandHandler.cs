using Mandorle.Application.Commands;
using Mandorle.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mandorle.Application.Handlers
{

    public class CreateProductCommandHandler //: IRequestHandler<CreateProductCommand, Product>
    {
        private readonly IProductRepository _repository;

        public CreateProductCommandHandler(IProductRepository repository)
        {
            _repository = repository;
        }

       
    }
}
