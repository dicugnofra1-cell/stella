using Mandorle.Application.Customers.Mapping;
using Mandorle.Application.Customers.Models;
using Mandorle.Application.Customers.Queries;
using Mandorle.Domain.Interfaces;
using MediatR;

namespace Mandorle.Application.Customers.Handlers;

public class GetCustomerAddressesQueryHandler : IRequestHandler<GetCustomerAddressesQuery, IReadOnlyList<CustomerAddressDto>>
{
    private readonly ICustomerRepository _repository;

    public GetCustomerAddressesQueryHandler(ICustomerRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<CustomerAddressDto>> Handle(GetCustomerAddressesQuery request, CancellationToken cancellationToken)
    {
        var addresses = await _repository.GetAddressesAsync(request.CustomerId, cancellationToken);
        return addresses.Select(address => address.ToDto()).ToList();
    }
}
