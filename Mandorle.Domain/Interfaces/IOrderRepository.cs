using Mandorle.Domain.Entities;

namespace Mandorle.Domain.Interfaces;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(int id, bool includeItems = false, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Order>> SearchAsync(
        int? customerId,
        string? orderType,
        string? status,
        string? paymentStatus,
        DateTime? fromDate,
        DateTime? toDate,
        CancellationToken cancellationToken = default);

    Task AddAsync(Order order, CancellationToken cancellationToken = default);

    void Update(Order order);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
