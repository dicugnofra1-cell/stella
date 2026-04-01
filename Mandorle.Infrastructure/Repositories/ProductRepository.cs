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

    public async Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken) =>
        await _context.Products
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public async Task<List<Product>> GetAllAsync(CancellationToken cancellationToken) =>
        await _context.Products
            .ToListAsync(cancellationToken);

    public async Task<int> AddAsync(Product product, CancellationToken cancellationToken)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync(cancellationToken);
        return product.Id;
    }
}