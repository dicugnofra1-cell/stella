using Mandorle.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mandorle.Domain.Interfaces
{

    public interface IProductRepository
    {
        Task<int> AddAsync(Product product, CancellationToken cancellationToken);
        Task<List<Product>> GetAllAsync(CancellationToken cancellationToken);
        Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken);
    }
}
