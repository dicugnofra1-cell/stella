using Mandorle.Application.Quality.Models;
using MediatR;

namespace Mandorle.Application.Quality.Commands;

public record UpdateNonConformityCommand(
    int Id,
    string Severity,
    string Status,
    string Description,
    string? CorrectiveAction) : IRequest<NonConformityDto?>;
