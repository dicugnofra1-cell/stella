namespace Mandorle.Application.Quality.Models;

public record NonConformityDto(
    int Id,
    int BatchId,
    string Severity,
    string Status,
    string Description,
    string? CorrectiveAction,
    string OpenedBy,
    DateTime OpenedAt,
    string? ClosedBy,
    DateTime? ClosedAt);
