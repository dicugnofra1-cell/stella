using Mandorle.Domain.Entities;
using Mandorle.Domain.Interfaces;
using Mandorle.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

public class ProductRepository : IProductRepository
{
    private readonly MandorleDbContext _context;

    public ProductRepository(MandorleDbContext context)
    {
        _context = context;
    }

    public async Task<Product?> GetByIdAsync(int id) =>
        await _context.Products.FindAsync(id);

    public async Task<List<Product>> GetAllAsync() =>
        await _context.Products.ToListAsync();

    public async Task AddAsync(Product product) =>
        await _context.Products.AddAsync(product);

    public async Task SaveChangesAsync() =>
        await _context.SaveChangesAsync();
}