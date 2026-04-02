using Mandorle.Application.Customers.Commands;
using Mandorle.Application.Customers.Mapping;
using Mandorle.Application.Customers.Models;
using Mandorle.Domain.Interfaces;
using MediatR;

namespace Mandorle.Application.Customers.Handlers;

public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, CustomerDto?>
{
    private readonly ICustomerRepository _repository;

    public UpdateCustomerCommandHandler(ICustomerRepository repository)
    {
        _repository = repository;
    }

    public async Task<CustomerDto?> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = await _repository.GetByIdAsync(request.Id, includeAddresses: true, cancellationToken);
        if (customer is null)
        {
            return null;
        }

        await EnsureUniqueFieldsAsync(request, cancellationToken);

        customer.Type = request.Type;
        customer.Name = request.Name;
        customer.VatNumber = request.VatNumber;
        customer.Email = request.Email;
        customer.Pec = request.Pec;
        customer.SpidIdentifier = request.SpidIdentifier;
        customer.Phone = request.Phone;
        customer.Status = request.Status;
        customer.UpdatedAt = DateTime.UtcNow;

        _repository.Update(customer);
        await _repository.SaveChangesAsync(cancellationToken);

        return customer.ToDto();
    }

    private async Task EnsureUniqueFieldsAsync(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        if (await _repository.ExistsByEmailAsync(request.Email, request.Id, cancellationToken))
        {
            throw new InvalidOperationException("A customer with the same email already exists.");
        }

        if (await _repository.ExistsByPecAsync(request.Pec, request.Id, cancellationToken))
        {
            throw new InvalidOperationException("A customer with the same PEC already exists.");
        }

        if (await _repository.ExistsBySpidIdentifierAsync(request.SpidIdentifier, request.Id, cancellationToken))
        {
            throw new InvalidOperationException("A customer with the same SPID identifier already exists.");
        }

        if (await _repository.ExistsByVatNumberAsync(request.VatNumber, request.Id, cancellationToken))
        {
            throw new InvalidOperationException("A customer with the same VAT number already exists.");
        }
    }
}
