using MediatR;

namespace Mandorle.Application.Quality.Commands;

public record CloseNonConformityCommand(
    int Id,
    string ClosedBy,
    DateTime? ClosedAt,
    string? CorrectiveAction) : IRequest<bool>;
