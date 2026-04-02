using Mandorle.Application.Customers.Mapping;
using Mandorle.Application.Customers.Models;
using Mandorle.Application.Customers.Queries;
using Mandorle.Domain.Interfaces;
using MediatR;

namespace Mandorle.Application.Customers.Handlers;

public class GetCustomerBySpidIdentifierQueryHandler : IRequestHandler<GetCustomerBySpidIdentifierQuery, CustomerDto?>
{
    private readonly ICustomerRepository _repository;

    public GetCustomerBySpidIdentifierQueryHandler(ICustomerRepository repository)
    {
        _repository = repository;
    }

    public async Task<CustomerDto?> Handle(GetCustomerBySpidIdentifierQuery request, CancellationToken cancellationToken)
    {
        var customer = await _repository.GetBySpidIdentifierAsync(request.SpidIdentifier, cancellationToken);
        return customer?.ToDto();
    }
}
