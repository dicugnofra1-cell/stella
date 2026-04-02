using Mandorle.Domain.Entities;

namespace Mandorle.Domain.Interfaces;

public interface IStockReservationRepository
{
    Task<StockReservation?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<StockReservation>> SearchAsync(
        int? orderId,
        int? orderItemId,
        int? productId,
        int? batchId,
        string? status,
        string? reservationType,
        CancellationToken cancellationToken = default);

    Task AddAsync(StockReservation stockReservation, CancellationToken cancellationToken = default);

    void Update(StockReservation stockReservation);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
