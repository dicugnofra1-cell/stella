using Mandorle.Application.GoodsReceipts.Abstractions;
using Mandorle.Application.GoodsReceipts.Models;
using Mandorle.Application.GoodsReceipts.Queries;
using MediatR;

namespace Mandorle.Application.GoodsReceipts.Handlers;

public class GetTodayGoodsReceiptsQueryHandler : IRequestHandler<GetTodayGoodsReceiptsQuery, IReadOnlyList<GoodsReceiptRowDto>>
{
    private readonly IGoodsReceiptReadRepository _repository;

    public GetTodayGoodsReceiptsQueryHandler(IGoodsReceiptReadRepository repository)
    {
        _repository = repository;
    }

    public Task<IReadOnlyList<GoodsReceiptRowDto>> Handle(GetTodayGoodsReceiptsQuery request, CancellationToken cancellationToken)
    {
        return _repository.GetTodayAsync(DateOnly.FromDateTime(DateTime.Now), request.Search, cancellationToken);
    }
}
