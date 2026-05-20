using Mandorle.Application.GoodsReceipts.Models;
using MediatR;

namespace Mandorle.Application.GoodsReceipts.Queries;

public record GetActiveGoodsReceiptsHistoryQuery(string? Search) : IRequest<IReadOnlyList<GoodsReceiptRowDto>>;
