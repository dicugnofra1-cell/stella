using Mandorle.Application.Customers.Commands;
using Mandorle.Application.Customers.Mapping;
using Mandorle.Application.Customers.Models;
using Mandorle.Domain.Entities;
using Mandorle.Domain.Interfaces;
using MediatR;

namespace Mandorle.Application.Customers.Handlers;

public class AddCustomerAddressCommandHandler : IRequestHandler<AddCustomerAddressCommand, CustomerAddressDto?>
{
    private readonly ICustomerRepository _repository;

    public AddCustomerAddressCommandHandler(ICustomerRepository repository)
    {
        _repository = repository;
    }

    public async Task<CustomerAddressDto?> Handle(AddCustomerAddressCommand request, CancellationToken cancellationToken)
    {
        var customer = await _repository.GetByIdAsync(request.CustomerId, cancellationToken: cancellationToken);
        if (customer is null)
        {
            return null;
        }

        var address = new CustomerAddress
        {
            CustomerId = request.CustomerId,
            AddressType = request.AddressType,
            RecipientName = request.RecipientName,
            Street = request.Street,
            Street2 = request.Street2,
            City = request.City,
            PostalCode = request.PostalCode,
            Province = request.Province,
            Country = request.Country,
            IsDefault = request.IsDefault,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddAddressAsync(address, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        if (request.IsDefault)
        {
            await _repository.SetDefaultAddressAsync(request.CustomerId, address.Id, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);
        }

        return address.ToDto();
    }
}
