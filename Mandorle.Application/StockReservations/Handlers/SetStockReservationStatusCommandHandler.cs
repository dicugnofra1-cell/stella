using Mandorle.Application.StockReservations.Commands;
using Mandorle.Domain.Enums;
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

        var normalizedStatus = Normalize(request.Status)!;

        reservation.Status = normalizedStatus;
        reservation.UpdatedAt = DateTime.UtcNow;

        if (normalizedStatus.Equals(StockReservationStatus.Released.ToDbValue(), StringComparison.OrdinalIgnoreCase))
        {
            reservation.ReleasedAt = DateTime.UtcNow;
        }

        if (normalizedStatus.Equals(StockReservationStatus.Consumed.ToDbValue(), StringComparison.OrdinalIgnoreCase))
        {
            reservation.ConsumedAt = DateTime.UtcNow;
        }

        _stockReservationRepository.Update(reservation);
        await _stockReservationRepository.SaveChangesAsync(cancellationToken);

        return true;
    }

    private static string? Normalize(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim().ToUpperInvariant();
    }
}
