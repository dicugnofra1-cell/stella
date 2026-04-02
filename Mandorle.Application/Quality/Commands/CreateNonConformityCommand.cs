using Mandorle.Application.Quality.Models;
using MediatR;

namespace Mandorle.Application.Quality.Commands;

public record CreateNonConformityCommand(
    int BatchId,
    string Severity,
    string Status,
    string Description,
    string? CorrectiveAction,
    string OpenedBy,
    DateTime? OpenedAt) : IRequest<NonConformityDto>;
