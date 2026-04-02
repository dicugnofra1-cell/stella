using Mandorle.Application.Quality.Models;
using MediatR;

namespace Mandorle.Application.Quality.Commands;

public record UpdateQualityCheckCommand(
    int Id,
    string ChecklistVersion,
    string Result,
    string? Notes,
    DateTime? CheckedAt,
    string CheckedBy) : IRequest<QualityCheckDto?>;
