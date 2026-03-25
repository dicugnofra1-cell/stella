using Mandorle.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mandorle.Domain.Interfaces
{

    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(int id);
        Task<List<Product>> GetAllAsync();
        Task AddAsync(Product product);
        Task SaveChangesAsync();
    }
}
