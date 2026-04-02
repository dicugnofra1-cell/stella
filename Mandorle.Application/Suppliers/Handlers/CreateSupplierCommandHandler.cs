using Mandorle.Application.Suppliers.Commands;
using Mandorle.Application.Suppliers.Mapping;
using Mandorle.Application.Suppliers.Models;
using Mandorle.Domain.Entities;
using Mandorle.Domain.Interfaces;
using MediatR;

namespace Mandorle.Application.Suppliers.Handlers;

public class CreateSupplierCommandHandler : IRequestHandler<CreateSupplierCommand, SupplierDto>
{
    private readonly ISupplierRepository _repository;

    public CreateSupplierCommandHandler(ISupplierRepository repository)
    {
        _repository = repository;
    }

    public async Task<SupplierDto> Handle(CreateSupplierCommand request, CancellationToken cancellationToken)
    {
        await EnsureUniqueFieldsAsync(request.Email, request.VatNumber, null, cancellationToken);

        var supplier = new Supplier
        {
            Name = request.Name,
            VatNumber = request.VatNumber,
            Address = request.Address,
            Email = request.Email,
            Phone = request.Phone,
            Status = request.Status,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(supplier, cancellationToken);
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
