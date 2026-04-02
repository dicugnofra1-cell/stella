using Mandorle.Application.Customers.Commands;
using Mandorle.Domain.Interfaces;
using MediatR;

namespace Mandorle.Application.Customers.Handlers;

public class SetCustomerStatusCommandHandler : IRequestHandler<SetCustomerStatusCommand, bool>
{
    private readonly ICustomerRepository _repository;

    public SetCustomerStatusCommandHandler(ICustomerRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(SetCustomerStatusCommand request, CancellationToken cancellationToken)
    {
        var customer = await _repository.GetByIdAsync(request.Id, cancellationToken: cancellationToken);
        if (customer is null)
        {
            return false;
        }

        customer.Status = request.Status;
        customer.UpdatedAt = DateTime.UtcNow;

        _repository.Update(customer);
        await _repository.SaveChangesAsync(cancellationToken);

        return true;
    }
}
