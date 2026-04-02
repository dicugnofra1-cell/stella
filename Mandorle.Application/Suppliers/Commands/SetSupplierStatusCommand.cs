using MediatR;

namespace Mandorle.Application.Suppliers.Commands;

public record SetSupplierStatusCommand(int Id, string Status) : IRequest<bool>;
