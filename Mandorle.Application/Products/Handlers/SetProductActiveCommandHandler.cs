using Mandorle.Application.Products.Commands;
using Mandorle.Domain.Interfaces;
using MediatR;

namespace Mandorle.Application.Products.Handlers;

public class SetProductActiveCommandHandler : IRequestHandler<SetProductActiveCommand, bool>
{
    private readonly IProductRepository _repository;

    public SetProductActiveCommandHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(SetProductActiveCommand request, CancellationToken cancellationToken)
    {
        var product = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (product is null)
        {
            return false;
        }

        product.Active = request.Active;
        product.UpdatedAt = DateTime.UtcNow;

        _repository.Update(product);
        await _repository.SaveChangesAsync(cancellationToken);

        return true;
    }
}
