using Mandorle.Application.Quality.Models;
using MediatR;

namespace Mandorle.Application.Quality.Queries;

public record GetNonConformityByIdQuery(int Id) : IRequest<NonConformityDto?>;
