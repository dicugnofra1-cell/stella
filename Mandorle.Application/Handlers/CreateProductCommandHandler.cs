using Mandorle.Application.Commands;
using Mandorle.Domain.Entities;
using Mandorle.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mandorle.Application.Handlers
{

    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Product>
    {
        private readonly IProductRepository _repository;

        public CreateProductCommandHandler(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task<Product> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var product = new Product
            {
                Name = request.Name,
                Sku = request.SKU
            };

            await _repository.AddAsync(product);
            await _repository.SaveChangesAsync();

            return product;
        }
    }
}
