using MediatR;

namespace Mandorle.Application.Products.Commands;

public record SetProductActiveCommand(int Id, bool Active) : IRequest<bool>;
