using Mandorle.Application.StockReservations.Models;
using MediatR;

namespace Mandorle.Application.StockReservations.Queries;

public record GetStockReservationByIdQuery(int Id) : IRequest<StockReservationDto?>;
