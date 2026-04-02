using Mandorle.Application.Traceability.Models;
using MediatR;

namespace Mandorle.Application.Traceability.Commands;

public record UpsertPublicTraceViewCommand(
    int BatchId,
    string BatchCode,
    string ProductName,
    bool BioFlag,
    string? OriginInfo,
    string? MainDates) : IRequest<PublicTraceViewDto>;
