using Mandorle.Application.StockReservations.Commands;
using Mandorle.Domain.Interfaces;
using MediatR;

namespace Mandorle.Application.StockReservations.Handlers;

public class SetStockReservationStatusCommandHandler : IRequestHandler<SetStockReservationStatusCommand, bool>
{
    private readonly IStockReservationRepository _stockReservationRepository;

    public SetStockReservationStatusCommandHandler(IStockReservationRepository stockReservationRepository)
    {
        _stockReservationRepository = stockReservationRepository;
    }

    public async Task<bool> Handle(SetStockReservationStatusCommand request, CancellationToken cancellationToken)
    {
        var reservation = await _stockReservationRepository.GetByIdAsync(request.Id, cancellationToken);
        if (reservation is null)
        {
            return false;
        }

        reservation.Status = request.Status;
        reservation.UpdatedAt = DateTime.UtcNow;

        if (request.Status.Equals("RELEASED", StringComparison.OrdinalIgnoreCase))
        {
            reservation.ReleasedAt = DateTime.UtcNow;
        }

        if (request.Status.Equals("CONSUMED", StringComparison.OrdinalIgnoreCase))
        {
            reservation.ConsumedAt = DateTime.UtcNow;
        }

        _stockReservationRepository.Update(reservation);
        await _stockReservationRepository.SaveChangesAsync(cancellationToken);

        return true;
    }
}
