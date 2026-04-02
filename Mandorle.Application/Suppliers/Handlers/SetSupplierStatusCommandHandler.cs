using Mandorle.Application.Suppliers.Commands;
using Mandorle.Domain.Interfaces;
using MediatR;

namespace Mandorle.Application.Suppliers.Handlers;

public class SetSupplierStatusCommandHandler : IRequestHandler<SetSupplierStatusCommand, bool>
{
    private readonly ISupplierRepository _repository;

    public SetSupplierStatusCommandHandler(ISupplierRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(SetSupplierStatusCommand request, CancellationToken cancellationToken)
    {
        var supplier = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (supplier is null)
        {
            return false;
        }

        supplier.Status = request.Status;
        supplier.UpdatedAt = DateTime.UtcNow;

        _repository.Update(supplier);
        await _repository.SaveChangesAsync(cancellationToken);

        return true;
    }
}
