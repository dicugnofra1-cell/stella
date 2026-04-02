using Mandorle.Application.Suppliers.Models;
using Mandorle.Application.Suppliers.Queries;
using Mandorle.Domain.Interfaces;
using MediatR;

namespace Mandorle.Application.Suppliers.Handlers;

public class CheckSupplierExistenceQueryHandler : IRequestHandler<CheckSupplierExistenceQuery, SupplierExistenceCheckResultDto>
{
    private readonly ISupplierRepository _repository;

    public CheckSupplierExistenceQueryHandler(ISupplierRepository repository)
    {
        _repository = repository;
    }

    public async Task<SupplierExistenceCheckResultDto> Handle(CheckSupplierExistenceQuery request, CancellationToken cancellationToken)
    {
        var emailExists = await _repository.ExistsByEmailAsync(request.Email, request.ExcludeSupplierId, cancellationToken);
        var vatNumberExists = await _repository.ExistsByVatNumberAsync(request.VatNumber, request.ExcludeSupplierId, cancellationToken);

        return new SupplierExistenceCheckResultDto(emailExists, vatNumberExists);
    }
}
