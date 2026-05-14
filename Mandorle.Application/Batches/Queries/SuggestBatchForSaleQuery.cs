using Mandorle.Application.Batches.Models;
using MediatR;

namespace Mandorle.Application.Batches.Queries;

public record SuggestBatchForSaleQuery(
    int ProductId,
    decimal Quantity,
    string? BatchType,
    bool? BioFlag) : IRequest<BatchSaleSuggestionDto?>;
