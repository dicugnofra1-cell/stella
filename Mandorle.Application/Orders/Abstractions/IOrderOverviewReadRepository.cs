using Mandorle.Application.Orders.Models;

namespace Mandorle.Application.Orders.Abstractions;

public interface IOrderOverviewReadRepository
{
    Task<IReadOnlyList<OrderOverviewRowDto>> SearchAsync(string? orderType, string? search, CancellationToken cancellationToken = default);
}
