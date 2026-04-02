using MediatR;

namespace Mandorle.Application.StockReservations.Commands;

public record SetStockReservationStatusCommand(int Id, string Status) : IRequest<bool>;
