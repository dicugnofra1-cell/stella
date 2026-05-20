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

        EnsureImmutableFields(batch, request);

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
        batch.Variety = request.Variety;
        batch.InitialQuantity = request.InitialQuantity;
        batch.UnitOfMeasure = request.UnitOfMeasure;
        batch.SupplierId = request.SupplierId;
        batch.SupplierDocumentId = request.SupplierDocumentId;
        batch.CertificationId = request.CertificationId;
        batch.ProductionDate = request.ProductionDate;
        batch.ExpirationDate = request.ExpirationDate;
        batch.Notes = request.Notes;
        batch.UpdatedAt = DateTime.UtcNow;

        _batchRepository.Update(batch);
        await _batchRepository.SaveChangesAsync(cancellationToken);

        return batch.ToDto();
    }

    private static void EnsureImmutableFields(Domain.Entities.Batch batch, UpdateBatchCommand request)
    {
        if (!string.Equals(batch.BatchCode, request.BatchCode, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Il codice lotto non puo essere modificato manualmente.");
        }

        if (batch.ProductId != request.ProductId)
        {
            throw new InvalidOperationException("Il prodotto del lotto deriva dall'ingresso merce e non puo essere modificato.");
        }

        if (!string.Equals(batch.BatchType, request.BatchType, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("La tipologia del lotto deriva dall'ingresso merce e non puo essere modificata.");
        }

        if (batch.BioFlag != request.BioFlag)
        {
            throw new InvalidOperationException("Il flag BIO del lotto deriva dall'ingresso merce e non puo essere modificato.");
        }

        if (batch.InitialQuantity != request.InitialQuantity)
        {
            throw new InvalidOperationException("La quantita iniziale del lotto deriva dall'ingresso merce e non puo essere modificata.");
        }

        if (!string.Equals(batch.UnitOfMeasure, request.UnitOfMeasure, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("L'unita di misura del lotto deriva dall'ingresso merce e non puo essere modificata.");
        }

        if (batch.SupplierId != request.SupplierId)
        {
            throw new InvalidOperationException("Il fornitore del lotto deriva dall'ingresso merce e non puo essere modificato.");
        }
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
