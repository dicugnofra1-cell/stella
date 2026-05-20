using Mandorle.Application.GoodsReceipts.Abstractions;
using Mandorle.Application.GoodsReceipts.Models;
using Mandorle.Application.GoodsReceipts.Queries;
using MediatR;

namespace Mandorle.Application.GoodsReceipts.Handlers;

public class GetActiveGoodsReceiptsHistoryQueryHandler : IRequestHandler<GetActiveGoodsReceiptsHistoryQuery, IReadOnlyList<GoodsReceiptRowDto>>
{
    private readonly IGoodsReceiptReadRepository _repository;

    public GetActiveGoodsReceiptsHistoryQueryHandler(IGoodsReceiptReadRepository repository)
    {
        _repository = repository;
    }

    public Task<IReadOnlyList<GoodsReceiptRowDto>> Handle(GetActiveGoodsReceiptsHistoryQuery request, CancellationToken cancellationToken)
    {
        return _repository.GetActiveHistoryAsync(request.Search, cancellationToken);
    }
}
