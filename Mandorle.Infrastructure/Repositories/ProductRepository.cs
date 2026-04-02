using Mandorle.Domain.Entities;
using Mandorle.Domain.Interfaces;
using Mandorle.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Mandorle.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly StellaFruttaDbContext _context;

    public ProductRepository(StellaFruttaDbContext context)
    {
        _context = context;
    }

    public Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _context.Products.FirstOrDefaultAsync(product => product.Id == id, cancellationToken);
    }

    public Task<Product?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default)
    {
        var normalizedSku = Normalize(sku);
        return _context.Products.FirstOrDefaultAsync(product => product.Sku == normalizedSku, cancellationToken);
    }

    public Task<bool> ExistsBySkuAsync(string sku, int? excludeProductId = null, CancellationToken cancellationToken = default)
    {
        var normalizedSku = Normalize(sku);

        return _context.Products.AnyAsync(
            product => product.Sku == normalizedSku && (!excludeProductId.HasValue || product.Id != excludeProductId.Value),
            cancellationToken);
    }

    public async Task<IReadOnlyList<Product>> SearchAsync(
        string? search,
        string? category,
        bool? active,
        bool? channelB2BEnabled,
        bool? channelB2CEnabled,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Products.AsNoTracking().AsQueryable();
        var normalizedSearch = Normalize(search);
        var normalizedCategory = Normalize(category);

        if (!string.IsNullOrWhiteSpace(normalizedCategory))
        {
            query = query.Where(product => product.Category == normalizedCategory);
        }

        if (active.HasValue)
        {
            query = query.Where(product => product.Active == active.Value);
        }

        if (channelB2BEnabled.HasValue)
        {
            query = query.Where(product => product.ChannelB2BEnabled == channelB2BEnabled.Value);
        }

        if (channelB2CEnabled.HasValue)
        {
            query = query.Where(product => product.ChannelB2CEnabled == channelB2CEnabled.Value);
        }

        if (!string.IsNullOrWhiteSpace(normalizedSearch))
        {
            query = query.Where(product =>
                product.Sku.Contains(normalizedSearch) ||
                product.Name.Contains(normalizedSearch) ||
                (product.Description != null && product.Description.Contains(normalizedSearch)) ||
                (product.Category != null && product.Category.Contains(normalizedSearch)) ||
                product.UnitOfMeasure.Contains(normalizedSearch));
        }

        return await query
            .OrderBy(product => product.Name)
            .ThenBy(product => product.Id)
            .ToListAsync(cancellationToken);
    }

    public Task AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        NormalizeProduct(product);
        return _context.Products.AddAsync(product, cancellationToken).AsTask();
    }

    public void Update(Product product)
    {
        NormalizeProduct(product);
        _context.Products.Update(product);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }

    private static void NormalizeProduct(Product product)
    {
        product.Sku = Normalize(product.Sku)!;
        product.Name = Normalize(product.Name)!;
        product.Description = Normalize(product.Description);
        product.UnitOfMeasure = Normalize(product.UnitOfMeasure)!;
        product.Category = Normalize(product.Category);
    }

    private static string? Normalize(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
