using Mandorle.Application.Batches.Models;
using MediatR;

namespace Mandorle.Application.Batches.Queries;

public record CheckBatchExistenceQuery(string BatchCode, int? ExcludeBatchId) : IRequest<BatchExistenceCheckResultDto>;
