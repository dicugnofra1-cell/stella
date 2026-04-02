using Mandorle.Application.Customers.Mapping;
using Mandorle.Application.Customers.Models;
using Mandorle.Application.Customers.Queries;
using Mandorle.Domain.Interfaces;
using MediatR;

namespace Mandorle.Application.Customers.Handlers;

public class GetDefaultCustomerAddressByTypeQueryHandler : IRequestHandler<GetDefaultCustomerAddressByTypeQuery, CustomerAddressDto?>
{
    private readonly ICustomerRepository _repository;

    public GetDefaultCustomerAddressByTypeQueryHandler(ICustomerRepository repository)
    {
        _repository = repository;
    }

    public async Task<CustomerAddressDto?> Handle(GetDefaultCustomerAddressByTypeQuery request, CancellationToken cancellationToken)
    {
        var address = await _repository.GetDefaultAddressByTypeAsync(request.CustomerId, request.AddressType, cancellationToken);
        return address?.ToDto();
    }
}
