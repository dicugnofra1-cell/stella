using Mandorle.Application.GoodsReceipts.Models;

namespace Mandorle.Application.GoodsReceipts.Abstractions;

public interface IGoodsReceiptReadRepository
{
    Task<IReadOnlyList<GoodsReceiptRowDto>> GetTodayAsync(DateOnly day, string? search, CancellationToken cancellationToken = default);
}
