using Mandorle.Application.Customers.Mapping;
using Mandorle.Application.Customers.Models;
using Mandorle.Application.Customers.Queries;
using Mandorle.Domain.Interfaces;
using MediatR;

namespace Mandorle.Application.Customers.Handlers;

public class GetCustomerByVatNumberQueryHandler : IRequestHandler<GetCustomerByVatNumberQuery, CustomerDto?>
{
    private readonly ICustomerRepository _repository;

    public GetCustomerByVatNumberQueryHandler(ICustomerRepository repository)
    {
        _repository = repository;
    }

    public async Task<CustomerDto?> Handle(GetCustomerByVatNumberQuery request, CancellationToken cancellationToken)
    {
        var customer = await _repository.GetByVatNumberAsync(request.VatNumber, cancellationToken);
        return customer?.ToDto();
    }
}
