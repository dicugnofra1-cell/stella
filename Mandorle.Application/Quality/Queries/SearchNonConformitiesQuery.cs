using Mandorle.Application.Quality.Models;
using MediatR;

namespace Mandorle.Application.Quality.Queries;

public record SearchNonConformitiesQuery(int? BatchId, string? Severity, string? Status) : IRequest<IReadOnlyList<NonConformityDto>>;
