using Mandorle.Application.Suppliers.Commands;
using Mandorle.Application.Suppliers.Mapping;
using Mandorle.Application.Suppliers.Models;
using Mandorle.Domain.Interfaces;
using MediatR;

namespace Mandorle.Application.Suppliers.Handlers;

public class UpdateSupplierCommandHandler : IRequestHandler<UpdateSupplierCommand, SupplierDto?>
{
    private readonly ISupplierRepository _repository;

    public UpdateSupplierCommandHandler(ISupplierRepository repository)
    {
        _repository = repository;
    }

    public async Task<SupplierDto?> Handle(UpdateSupplierCommand request, CancellationToken cancellationToken)
    {
        var supplier = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (supplier is null)
        {
            return null;
        }

        await EnsureUniqueFieldsAsync(request.Email, request.VatNumber, request.Id, cancellationToken);

        supplier.Name = request.Name;
        supplier.VatNumber = request.VatNumber;
        supplier.Address = request.Address;
        supplier.Email = request.Email;
        supplier.Phone = request.Phone;
        supplier.Status = request.Status;
        supplier.UpdatedAt = DateTime.UtcNow;

        _repository.Update(supplier);
        await _repository.SaveChangesAsync(cancellationToken);

        return supplier.ToDto();
    }

    private async Task EnsureUniqueFieldsAsync(string? email, string? vatNumber, int? excludeSupplierId, CancellationToken cancellationToken)
    {
        if (await _repository.ExistsByEmailAsync(email, excludeSupplierId, cancellationToken))
        {
            throw new InvalidOperationException("A supplier with the same email already exists.");
        }

        if (await _repository.ExistsByVatNumberAsync(vatNumber, excludeSupplierId, cancellationToken))
        {
            throw new InvalidOperationException("A supplier with the same VAT number already exists.");
        }
    }
}
