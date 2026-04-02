using MediatR;

namespace Mandorle.Application.Batches.Commands;

public record SetBatchStatusCommand(int Id, string Status) : IRequest<bool>;
