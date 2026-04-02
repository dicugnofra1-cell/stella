using Mandorle.Domain.Entities;
using Mandorle.Domain.Interfaces;
using Mandorle.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Mandorle.Infrastructure.Repositories;

public class OrderItemRepository : IOrderItemRepository
{
    private readonly StellaFruttaDbContext _context;

    public OrderItemRepository(StellaFruttaDbContext context)
    {
        _context = context;
    }

    public Task<OrderItem?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _context.OrderItems.FirstOrDefaultAsync(orderItem => orderItem.Id == id, cancellationToken);
    }

    public Task<OrderItem?> GetByIdAsync(int orderId, int id, CancellationToken cancellationToken = default)
    {
        return _context.OrderItems.FirstOrDefaultAsync(
            orderItem => orderItem.OrderId == orderId && orderItem.Id == id,
            cancellationToken);
    }

    public async Task<IReadOnlyList<OrderItem>> GetByOrderIdAsync(int orderId, CancellationToken cancellationToken = default)
    {
        return await _context.OrderItems
            .AsNoTracking()
            .Where(orderItem => orderItem.OrderId == orderId)
            .OrderBy(orderItem => orderItem.Id)
            .ToListAsync(cancellationToken);
    }

    public Task AddAsync(OrderItem orderItem, CancellationToken cancellationToken = default)
    {
        return _context.OrderItems.AddAsync(orderItem, cancellationToken).AsTask();
    }

    public void Update(OrderItem orderItem)
    {
        _context.OrderItems.Update(orderItem);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}
