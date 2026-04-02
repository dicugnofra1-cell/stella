using Mandorle.Domain.Entities;

namespace Mandorle.Domain.Interfaces;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<Product?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default);

    Task<bool> ExistsBySkuAsync(string sku, int? excludeProductId = null, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Product>> SearchAsync(
        string? search,
        string? category,
        bool? active,
        bool? channelB2BEnabled,
        bool? channelB2CEnabled,
        CancellationToken cancellationToken = default);

    Task AddAsync(Product product, CancellationToken cancellationToken = default);

    void Update(Product product);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
