using Mandorle.Application.Customers.Commands;
using Mandorle.Application.Customers.Mapping;
using Mandorle.Application.Customers.Models;
using Mandorle.Domain.Entities;
using Mandorle.Domain.Interfaces;
using MediatR;

namespace Mandorle.Application.Customers.Handlers;

public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, CustomerDto>
{
    private readonly ICustomerRepository _repository;

    public CreateCustomerCommandHandler(ICustomerRepository repository)
    {
        _repository = repository;
    }

    public async Task<CustomerDto> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        await EnsureUniqueFieldsAsync(request.Email, request.Pec, request.SpidIdentifier, request.VatNumber, null, cancellationToken);

        var customer = new Customer
        {
            Type = request.Type,
            Name = request.Name,
            VatNumber = request.VatNumber,
            Email = request.Email,
            Pec = request.Pec,
            SpidIdentifier = request.SpidIdentifier,
            Phone = request.Phone,
            Status = request.Status,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(customer, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return customer.ToDto();
    }

    private async Task EnsureUniqueFieldsAsync(string email, string? pec, string? spidIdentifier, string? vatNumber, int? excludeCustomerId, CancellationToken cancellationToken)
    {
        if (await _repository.ExistsByEmailAsync(email, excludeCustomerId, cancellationToken))
        {
            throw new InvalidOperationException("A customer with the same email already exists.");
        }

        if (await _repository.ExistsByPecAsync(pec, excludeCustomerId, cancellationToken))
        {
            throw new InvalidOperationException("A customer with the same PEC already exists.");
        }

        if (await _repository.ExistsBySpidIdentifierAsync(spidIdentifier, excludeCustomerId, cancellationToken))
        {
            throw new InvalidOperationException("A customer with the same SPID identifier already exists.");
        }

        if (await _repository.ExistsByVatNumberAsync(vatNumber, excludeCustomerId, cancellationToken))
        {
            throw new InvalidOperationException("A customer with the same VAT number already exists.");
        }
    }
}
