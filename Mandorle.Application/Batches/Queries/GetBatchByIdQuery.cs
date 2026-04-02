using Mandorle.Application.Batches.Models;
using MediatR;

namespace Mandorle.Application.Batches.Queries;

public record GetBatchByIdQuery(int Id) : IRequest<BatchDto?>;
