using Mandorle.Domain.Entities;
using Mandorle.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mandorle.Application.Products.Commands.CreateProduct
{

    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, int>
    {
        private readonly IProductRepository _repository;

        public CreateProductCommandHandler(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task<int> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var product = new Product
            {
                Name = request.Name,
                Sku = request.SKU
            };

            return await _repository.AddAsync(product, cancellationToken);
        }
    }
}
