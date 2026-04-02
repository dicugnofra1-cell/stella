using Mandorle.Application.Batches.Models;
using MediatR;

namespace Mandorle.Application.Batches.Queries;

public record GetBatchLinkByIdQuery(int ChildBatchId, int BatchLinkId) : IRequest<BatchLinkDto?>;
