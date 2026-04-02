using Mandorle.Application.Customers.Mapping;
using Mandorle.Application.Customers.Models;
using Mandorle.Application.Customers.Queries;
using Mandorle.Domain.Interfaces;
using MediatR;

namespace Mandorle.Application.Customers.Handlers;

public class GetCustomerByPecQueryHandler : IRequestHandler<GetCustomerByPecQuery, CustomerDto?>
{
    private readonly ICustomerRepository _repository;

    public GetCustomerByPecQueryHandler(ICustomerRepository repository)
    {
        _repository = repository;
    }

    public async Task<CustomerDto?> Handle(GetCustomerByPecQuery request, CancellationToken cancellationToken)
    {
        var customer = await _repository.GetByPecAsync(request.Pec, cancellationToken);
        return customer?.ToDto();
    }
}
