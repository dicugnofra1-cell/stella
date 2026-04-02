using Mandorle.Application.StockReservations.Mapping;
using Mandorle.Application.StockReservations.Models;
using Mandorle.Application.StockReservations.Queries;
using Mandorle.Domain.Interfaces;
using MediatR;

namespace Mandorle.Application.StockReservations.Handlers;

public class SearchStockReservationsQueryHandler : IRequestHandler<SearchStockReservationsQuery, IReadOnlyList<StockReservationDto>>
{
    private readonly IStockReservationRepository _stockReservationRepository;

    public SearchStockReservationsQueryHandler(IStockReservationRepository stockReservationRepository)
    {
        _stockReservationRepository = stockReservationRepository;
    }

    public async Task<IReadOnlyList<StockReservationDto>> Handle(SearchStockReservationsQuery request, CancellationToken cancellationToken)
    {
        var reservations = await _stockReservationRepository.SearchAsync(
            request.OrderId,
            request.OrderItemId,
            request.ProductId,
            request.BatchId,
            request.Status,
            request.ReservationType,
            cancellationToken);

        return reservations.Select(reservation => reservation.ToDto()).ToList();
    }
}
