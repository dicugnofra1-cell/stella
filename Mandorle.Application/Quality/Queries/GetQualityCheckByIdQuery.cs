using Mandorle.Application.Quality.Models;
using MediatR;

namespace Mandorle.Application.Quality.Queries;

public record GetQualityCheckByIdQuery(int Id) : IRequest<QualityCheckDto?>;
