using Mandorle.Application.Batches.Models;
using MediatR;

namespace Mandorle.Application.Batches.Queries;

public record GetBatchByCodeQuery(string BatchCode) : IRequest<BatchDto?>;
