using Mandorle.Application.Batches.Models;
using MediatR;

namespace Mandorle.Application.Batches.Queries;

public record SearchBatchesQuery(
    string? Search,
    int? ProductId,
    int? SupplierId,
    string? BatchType,
    string? Status,
    bool? BioFlag) : IRequest<IReadOnlyList<BatchDto>>;
