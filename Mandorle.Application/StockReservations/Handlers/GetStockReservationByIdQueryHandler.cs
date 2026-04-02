using Mandorle.Application.StockReservations.Mapping;
using Mandorle.Application.StockReservations.Models;
using Mandorle.Application.StockReservations.Queries;
using Mandorle.Domain.Interfaces;
using MediatR;

namespace Mandorle.Application.StockReservations.Handlers;

public class GetStockReservationByIdQueryHandler : IRequestHandler<GetStockReservationByIdQuery, StockReservationDto?>
{
    private readonly IStockReservationRepository _stockReservationRepository;

    public GetStockReservationByIdQueryHandler(IStockReservationRepository stockReservationRepository)
    {
        _stockReservationRepository = stockReservationRepository;
    }

    public async Task<StockReservationDto?> Handle(GetStockReservationByIdQuery request, CancellationToken cancellationToken)
    {
        var reservation = await _stockReservationRepository.GetByIdAsync(request.Id, cancellationToken);
        return reservation?.ToDto();
    }
}
