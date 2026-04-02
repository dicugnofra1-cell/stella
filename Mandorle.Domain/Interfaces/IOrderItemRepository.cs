using Mandorle.Domain.Entities;

namespace Mandorle.Domain.Interfaces;

public interface IOrderItemRepository
{
    Task<OrderItem?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<OrderItem?> GetByIdAsync(int orderId, int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<OrderItem>> GetByOrderIdAsync(int orderId, CancellationToken cancellationToken = default);

    Task AddAsync(OrderItem orderItem, CancellationToken cancellationToken = default);

    void Update(OrderItem orderItem);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
