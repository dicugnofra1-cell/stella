namespace Mandorle.Application.Quality.Models;

public record QualityCheckDto(
    int Id,
    int BatchId,
    string ChecklistVersion,
    string Result,
    string? Notes,
    DateTime CheckedAt,
    string CheckedBy);
