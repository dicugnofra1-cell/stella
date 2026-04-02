using Mandorle.Domain.Entities;
using Mandorle.Domain.Interfaces;
using Mandorle.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Mandorle.Infrastructure.Repositories;

public class StockReservationRepository : IStockReservationRepository
{
    private readonly StellaFruttaDbContext _context;

    public StockReservationRepository(StellaFruttaDbContext context)
    {
        _context = context;
    }

    public Task<StockReservation?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _context.StockReservations.FirstOrDefaultAsync(reservation => reservation.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<StockReservation>> SearchAsync(
        int? orderId,
        int? orderItemId,
        int? productId,
        int? batchId,
        string? status,
        string? reservationType,
        CancellationToken cancellationToken = default)
    {
        var query = _context.StockReservations.AsNoTracking().AsQueryable();
        var normalizedStatus = Normalize(status);
        var normalizedReservationType = Normalize(reservationType);

        if (orderId.HasValue)
        {
            query = query.Where(reservation => reservation.OrderId == orderId.Value);
        }

        if (orderItemId.HasValue)
        {
            query = query.Where(reservation => reservation.OrderItemId == orderItemId.Value);
        }

        if (productId.HasValue)
        {
            query = query.Where(reservation => reservation.ProductId == productId.Value);
        }

        if (batchId.HasValue)
        {
            query = query.Where(reservation => reservation.BatchId == batchId.Value);
        }

        if (!string.IsNullOrWhiteSpace(normalizedStatus))
        {
            query = query.Where(reservation => reservation.Status == normalizedStatus);
        }

        if (!string.IsNullOrWhiteSpace(normalizedReservationType))
        {
            query = query.Where(reservation => reservation.ReservationType == normalizedReservationType);
        }

        return await query
            .OrderByDescending(reservation => reservation.CreatedAt)
            .ThenByDescending(reservation => reservation.Id)
            .ToListAsync(cancellationToken);
    }

    public Task AddAsync(StockReservation stockReservation, CancellationToken cancellationToken = default)
    {
        NormalizeReservation(stockReservation);
        return _context.StockReservations.AddAsync(stockReservation, cancellationToken).AsTask();
    }

    public void Update(StockReservation stockReservation)
    {
        NormalizeReservation(stockReservation);
        _context.StockReservations.Update(stockReservation);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }

    private static void NormalizeReservation(StockReservation stockReservation)
    {
        stockReservation.Status = Normalize(stockReservation.Status)!;
        stockReservation.ReservationType = Normalize(stockReservation.ReservationType)!;
        stockReservation.Notes = Normalize(stockReservation.Notes);
    }

    private static string? Normalize(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim().ToUpperInvariant();
    }
}
