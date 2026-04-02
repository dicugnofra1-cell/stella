using Mandorle.Application.Batches.Commands;
using Mandorle.Application.Batches.Mapping;
using Mandorle.Application.Batches.Models;
using Mandorle.Domain.Interfaces;
using MediatR;

namespace Mandorle.Application.Batches.Handlers;

public class UpdateBatchCommandHandler : IRequestHandler<UpdateBatchCommand, BatchDto?>
{
    private readonly IBatchRepository _batchRepository;
    private readonly IProductRepository _productRepository;
    private readonly ISupplierRepository _supplierRepository;

    public UpdateBatchCommandHandler(
        IBatchRepository batchRepository,
        IProductRepository productRepository,
        ISupplierRepository supplierRepository)
    {
        _batchRepository = batchRepository;
        _productRepository = productRepository;
        _supplierRepository = supplierRepository;
    }

    public async Task<BatchDto?> Handle(UpdateBatchCommand request, CancellationToken cancellationToken)
    {
        var batch = await _batchRepository.GetByIdAsync(request.Id, cancellationToken);
        if (batch is null)
        {
            return null;
        }

        if (await _batchRepository.ExistsByBatchCodeAsync(request.BatchCode, request.Id, cancellationToken))
        {
            throw new InvalidOperationException("A batch with the same batch code already exists.");
        }

        await EnsureReferencesExistAsync(request.ProductId, request.SupplierId, cancellationToken);

        batch.BatchCode = request.BatchCode;
        batch.ProductId = request.ProductId;
        batch.BatchType = request.BatchType;
        batch.Status = request.Status;
        batch.BioFlag = request.BioFlag;
        batch.SupplierId = request.SupplierId;
        batch.ProductionDate = request.ProductionDate;
        batch.ExpirationDate = request.ExpirationDate;
        batch.Notes = request.Notes;
        batch.UpdatedAt = DateTime.UtcNow;

        _batchRepository.Update(batch);
        await _batchRepository.SaveChangesAsync(cancellationToken);

        return batch.ToDto();
    }

    private async Task EnsureReferencesExistAsync(int productId, int? supplierId, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
        if (product is null)
        {
            throw new InvalidOperationException("The selected product does not exist.");
        }

        if (supplierId.HasValue)
        {
            var supplier = await _supplierRepository.GetByIdAsync(supplierId.Value, cancellationToken);
            if (supplier is null)
            {
                throw new InvalidOperationException("The selected supplier does not exist.");
            }
        }
    }
}
