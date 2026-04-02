using Mandorle.Application.Customers.Commands;
using Mandorle.Domain.Interfaces;
using MediatR;

namespace Mandorle.Application.Customers.Handlers;

public class SetDefaultCustomerAddressCommandHandler : IRequestHandler<SetDefaultCustomerAddressCommand, bool>
{
    private readonly ICustomerRepository _repository;

    public SetDefaultCustomerAddressCommandHandler(ICustomerRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(SetDefaultCustomerAddressCommand request, CancellationToken cancellationToken)
    {
        var address = await _repository.GetAddressByIdAsync(request.CustomerId, request.AddressId, cancellationToken);
        if (address is null)
        {
            return false;
        }

        await _repository.SetDefaultAddressAsync(request.CustomerId, request.AddressId, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return true;
    }
}
