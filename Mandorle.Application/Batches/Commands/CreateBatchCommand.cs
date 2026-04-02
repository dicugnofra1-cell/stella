using Mandorle.Application.Batches.Models;
using MediatR;

namespace Mandorle.Application.Batches.Commands;

public record CreateBatchCommand(
    string BatchCode,
    int ProductId,
    string BatchType,
    string Status,
    bool BioFlag,
    int? SupplierId,
    DateOnly? ProductionDate,
    DateOnly? ExpirationDate,
    string? Notes) : IRequest<BatchDto>;
