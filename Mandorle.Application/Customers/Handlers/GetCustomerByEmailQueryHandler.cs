using Mandorle.Application.Customers.Mapping;
using Mandorle.Application.Customers.Models;
using Mandorle.Application.Customers.Queries;
using Mandorle.Domain.Interfaces;
using MediatR;

namespace Mandorle.Application.Customers.Handlers;

public class GetCustomerByEmailQueryHandler : IRequestHandler<GetCustomerByEmailQuery, CustomerDto?>
{
    private readonly ICustomerRepository _repository;

    public GetCustomerByEmailQueryHandler(ICustomerRepository repository)
    {
        _repository = repository;
    }

    public async Task<CustomerDto?> Handle(GetCustomerByEmailQuery request, CancellationToken cancellationToken)
    {
        var customer = await _repository.GetByEmailAsync(request.Email, cancellationToken);
        return customer?.ToDto();
    }
}
