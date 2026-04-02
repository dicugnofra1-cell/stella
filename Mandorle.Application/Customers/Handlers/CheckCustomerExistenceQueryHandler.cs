using Mandorle.Application.Customers.Models;
using Mandorle.Application.Customers.Queries;
using Mandorle.Domain.Interfaces;
using MediatR;

namespace Mandorle.Application.Customers.Handlers;

public class CheckCustomerExistenceQueryHandler : IRequestHandler<CheckCustomerExistenceQuery, CustomerExistenceCheckResultDto>
{
    private readonly ICustomerRepository _repository;

    public CheckCustomerExistenceQueryHandler(ICustomerRepository repository)
    {
        _repository = repository;
    }

    public async Task<CustomerExistenceCheckResultDto> Handle(CheckCustomerExistenceQuery request, CancellationToken cancellationToken)
    {
        var emailExists = !string.IsNullOrWhiteSpace(request.Email)
            && await _repository.ExistsByEmailAsync(request.Email, request.ExcludeCustomerId, cancellationToken);

        var pecExists = await _repository.ExistsByPecAsync(request.Pec, request.ExcludeCustomerId, cancellationToken);
        var spidExists = await _repository.ExistsBySpidIdentifierAsync(request.SpidIdentifier, request.ExcludeCustomerId, cancellationToken);
        var vatExists = await _repository.ExistsByVatNumberAsync(request.VatNumber, request.ExcludeCustomerId, cancellationToken);

        return new CustomerExistenceCheckResultDto(emailExists, pecExists, spidExists, vatExists);
    }
}
