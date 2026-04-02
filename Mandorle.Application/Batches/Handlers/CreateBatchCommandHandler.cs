using Mandorle.Application.Batches.Commands;
using Mandorle.Application.Batches.Mapping;
using Mandorle.Application.Batches.Models;
using Mandorle.Domain.Entities;
using Mandorle.Domain.Interfaces;
using MediatR;

namespace Mandorle.Application.Batches.Handlers;

public class CreateBatchCommandHandler : IRequestHandler<CreateBatchCommand, BatchDto>
{
    private readonly IBatchRepository _batchRepository;
    private readonly IProductRepository _productRepository;
    private readonly ISupplierRepository _supplierRepository;

    public CreateBatchCommandHandler(
        IBatchRepository batchRepository,
        IProductRepository productRepository,
        ISupplierRepository supplierRepository)
    {
        _batchRepository = batchRepository;
        _productRepository = productRepository;
        _supplierRepository = supplierRepository;
    }

    public async Task<BatchDto> Handle(CreateBatchCommand request, CancellationToken cancellationToken)
    {
        if (await _batchRepository.ExistsByBatchCodeAsync(request.BatchCode, cancellationToken: cancellationToken))
        {
            throw new InvalidOperationException("A batch with the same batch code already exists.");
        }

        await EnsureReferencesExistAsync(request.ProductId, request.SupplierId, cancellationToken);

        var batch = new Batch
        {
            BatchCode = request.BatchCode,
            ProductId = request.ProductId,
            BatchType = request.BatchType,
            Status = request.Status,
            BioFlag = request.BioFlag,
            SupplierId = request.SupplierId,
            ProductionDate = request.ProductionDate,
            ExpirationDate = request.ExpirationDate,
            Notes = request.Notes,
            CreatedAt = DateTime.UtcNow
        };

        await _batchRepository.AddAsync(batch, cancellationToken);
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
