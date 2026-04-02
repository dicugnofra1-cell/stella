using Mandorle.Application.Customers.Commands;
using Mandorle.Application.Customers.Mapping;
using Mandorle.Application.Customers.Models;
using Mandorle.Domain.Interfaces;
using MediatR;

namespace Mandorle.Application.Customers.Handlers;

public class UpdateCustomerAddressCommandHandler : IRequestHandler<UpdateCustomerAddressCommand, CustomerAddressDto?>
{
    private readonly ICustomerRepository _repository;

    public UpdateCustomerAddressCommandHandler(ICustomerRepository repository)
    {
        _repository = repository;
    }

    public async Task<CustomerAddressDto?> Handle(UpdateCustomerAddressCommand request, CancellationToken cancellationToken)
    {
        var address = await _repository.GetAddressByIdAsync(request.CustomerId, request.AddressId, cancellationToken);
        if (address is null)
        {
            return null;
        }

        address.AddressType = request.AddressType;
        address.RecipientName = request.RecipientName;
        address.Street = request.Street;
        address.Street2 = request.Street2;
        address.City = request.City;
        address.PostalCode = request.PostalCode;
        address.Province = request.Province;
        address.Country = request.Country;
        address.IsDefault = request.IsDefault;
        address.UpdatedAt = DateTime.UtcNow;

        _repository.UpdateAddress(address);
        await _repository.SaveChangesAsync(cancellationToken);

        if (request.IsDefault)
        {
            await _repository.SetDefaultAddressAsync(request.CustomerId, request.AddressId, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);
        }

        return address.ToDto();
    }
}
