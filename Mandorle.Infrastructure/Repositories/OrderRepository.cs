using Mandorle.Domain.Entities;
using Mandorle.Domain.Interfaces;
using Mandorle.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Mandorle.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly StellaFruttaDbContext _context;

    public OrderRepository(StellaFruttaDbContext context)
    {
        _context = context;
    }

    public Task<Order?> GetByIdAsync(int id, bool includeItems = false, CancellationToken cancellationToken = default)
    {
        IQueryable<Order> query = _context.Orders;

        if (includeItems)
        {
            query = query.Include(order => order.OrderItems);
        }

        return query.FirstOrDefaultAsync(order => order.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Order>> SearchAsync(
        int? customerId,
        string? orderType,
        string? status,
        string? paymentStatus,
        DateTime? fromDate,
        DateTime? toDate,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Orders.AsNoTracking().AsQueryable();
        var normalizedOrderType = Normalize(orderType);
        var normalizedStatus = Normalize(status);
        var normalizedPaymentStatus = Normalize(paymentStatus);

        if (customerId.HasValue)
        {
            query = query.Where(order => order.CustomerId == customerId.Value);
        }

        if (!string.IsNullOrWhiteSpace(normalizedOrderType))
        {
            query = query.Where(order => order.OrderType == normalizedOrderType);
        }

        if (!string.IsNullOrWhiteSpace(normalizedStatus))
        {
            query = query.Where(order => order.Status == normalizedStatus);
        }

        if (!string.IsNullOrWhiteSpace(normalizedPaymentStatus))
        {
            query = query.Where(order => order.PaymentStatus == normalizedPaymentStatus);
        }

        if (fromDate.HasValue)
        {
            query = query.Where(order => order.CreatedAt >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            query = query.Where(order => order.CreatedAt <= toDate.Value);
        }

        return await query
            .OrderByDescending(order => order.CreatedAt)
            .ThenByDescending(order => order.Id)
            .ToListAsync(cancellationToken);
    }

    public Task AddAsync(Order order, CancellationToken cancellationToken = default)
    {
        NormalizeOrder(order);
        return _context.Orders.AddAsync(order, cancellationToken).AsTask();
    }

    public void Update(Order order)
    {
        NormalizeOrder(order);
        _context.Orders.Update(order);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }

    private static void NormalizeOrder(Order order)
    {
        order.OrderType = Normalize(order.OrderType)!;
        order.Status = Normalize(order.Status)!;
        order.PaymentStatus = Normalize(order.PaymentStatus);
        order.Currency = Normalize(order.Currency)!;
        order.Notes = Normalize(order.Notes);
    }

    private static string? Normalize(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim().ToUpperInvariant();
    }
}
