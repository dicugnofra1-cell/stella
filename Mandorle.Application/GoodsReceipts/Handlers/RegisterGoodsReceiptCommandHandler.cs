using Mandorle.Application.Batches.Mapping;
using Mandorle.Application.GoodsReceipts.Commands;
using Mandorle.Application.GoodsReceipts.Models;
using Mandorle.Application.Inventory.Mapping;
using Mandorle.Domain.Entities;
using Mandorle.Domain.Enums;
using Mandorle.Domain.Interfaces;
using MediatR;

namespace Mandorle.Application.GoodsReceipts.Handlers;

public class RegisterGoodsReceiptCommandHandler : IRequestHandler<RegisterGoodsReceiptCommand, GoodsReceiptDto>
{
    private readonly IBatchRepository _batchRepository;
    private readonly IProductRepository _productRepository;
    private readonly ISupplierRepository _supplierRepository;
    private readonly ISupplierDocumentRepository _supplierDocumentRepository;
    private readonly ICertificationRepository _certificationRepository;
    private readonly IInventoryMovementRepository _inventoryMovementRepository;

    public RegisterGoodsReceiptCommandHandler(
        IBatchRepository batchRepository,
        IProductRepository productRepository,
        ISupplierRepository supplierRepository,
        ISupplierDocumentRepository supplierDocumentRepository,
        ICertificationRepository certificationRepository,
        IInventoryMovementRepository inventoryMovementRepository)
    {
        _batchRepository = batchRepository;
        _productRepository = productRepository;
        _supplierRepository = supplierRepository;
        _supplierDocumentRepository = supplierDocumentRepository;
        _certificationRepository = certificationRepository;
        _inventoryMovementRepository = inventoryMovementRepository;
    }

    public async Task<GoodsReceiptDto> Handle(RegisterGoodsReceiptCommand request, CancellationToken cancellationToken)
    {
        if (request.Quantity <= 0)
        {
            throw new InvalidOperationException("La quantita deve essere maggiore di zero.");
        }

        if (string.IsNullOrWhiteSpace(request.UnitOfMeasure))
        {
            throw new InvalidOperationException("L'unita di misura e obbligatoria.");
        }

        if (string.IsNullOrWhiteSpace(request.UserId))
        {
            throw new InvalidOperationException("L'utente che registra l'ingresso e obbligatorio.");
        }

        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken)
            ?? throw new InvalidOperationException("Il prodotto selezionato non esiste.");

        var supplier = await _supplierRepository.GetByIdAsync(request.SupplierId, cancellationToken)
            ?? throw new InvalidOperationException("Il fornitore selezionato non esiste.");

        var supplierDocument = await ValidateSupplierDocumentAsync(request.SupplierId, request.SupplierDocumentId, cancellationToken);
        var certification = await ValidateCertificationAsync(request, cancellationToken);

        var batchCode = await GenerateBatchCodeAsync(cancellationToken);

        var batch = new Batch
        {
            BatchCode = batchCode,
            ProductId = product.Id,
            BatchType = request.BatchType,
            Status = BatchStatus.Ricevuto.ToDbValue(),
            BioFlag = request.BioFlag,
            Variety = Normalize(request.Variety),
            InitialQuantity = request.Quantity,
            UnitOfMeasure = request.UnitOfMeasure.Trim(),
            SupplierId = supplier.Id,
            SupplierDocumentId = supplierDocument?.Id,
            CertificationId = certification?.Id,
            ProductionDate = request.ProductionDate,
            ExpirationDate = request.ExpirationDate,
            Notes = Normalize(request.Notes),
            CreatedAt = DateTime.UtcNow
        };

        await _batchRepository.AddAsync(batch, cancellationToken);

        var movement = new InventoryMovement
        {
            ProductId = product.Id,
            Batch = batch,
            MovementType = InventoryMovementType.Load.ToDbValue(),
            Quantity = request.Quantity,
            MovementDate = request.ReceivedAt ?? DateTime.UtcNow,
            Reason = "INGRESSO_MERCE",
            ReferenceType = MovementReferenceType.GoodsReceipt.ToDbValue(),
            ReferenceId = batchCode,
            UserId = request.UserId,
            CreatedAt = DateTime.UtcNow
        };

        await _inventoryMovementRepository.AddAsync(movement, cancellationToken);
        await _batchRepository.SaveChangesAsync(cancellationToken);

        return new GoodsReceiptDto(
            batch.ToDto(),
            movement.ToDto(),
            request.Quantity,
            request.UnitOfMeasure.Trim(),
            supplierDocument?.Id,
            certification?.Id);
    }

    private async Task<SupplierDocument?> ValidateSupplierDocumentAsync(int supplierId, int? supplierDocumentId, CancellationToken cancellationToken)
    {
        if (!supplierDocumentId.HasValue)
        {
            return null;
        }

        var supplierDocument = await _supplierDocumentRepository.GetByIdAsync(supplierId, supplierDocumentId.Value, cancellationToken);
        if (supplierDocument is null)
        {
            throw new InvalidOperationException("Il documento fornitore selezionato non esiste o non appartiene al fornitore.");
        }

        return supplierDocument;
    }

    private async Task<Certification?> ValidateCertificationAsync(RegisterGoodsReceiptCommand request, CancellationToken cancellationToken)
    {
        Certification? certification = null;

        if (request.CertificationId.HasValue)
        {
            certification = await _certificationRepository.GetByIdAsync(request.SupplierId, request.CertificationId.Value, cancellationToken);
            if (certification is null)
            {
                throw new InvalidOperationException("La certificazione selezionata non esiste o non appartiene al fornitore.");
            }

            if (!string.Equals(certification.Status, CertificationStatus.Active.ToDbValue(), StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("La certificazione selezionata non e attiva.");
            }

            if (certification.ExpiryDate < DateOnly.FromDateTime(DateTime.UtcNow))
            {
                throw new InvalidOperationException("La certificazione selezionata e scaduta.");
            }
        }

        if (request.BioFlag && certification is null)
        {
            throw new InvalidOperationException("Per un ingresso BIO e obbligatoria una certificazione valida.");
        }

        if (request.SupplierDocumentId.HasValue && certification?.DocumentId.HasValue == true && certification.DocumentId.Value != request.SupplierDocumentId.Value)
        {
            throw new InvalidOperationException("Il documento selezionato non corrisponde alla certificazione indicata.");
        }

        return certification;
    }

    private async Task<string> GenerateBatchCodeAsync(CancellationToken cancellationToken)
    {
        for (var attempt = 0; attempt < 10; attempt++)
        {
            var candidate = $"LOTTO-{DateTime.UtcNow:yyyyMMdd}-{Random.Shared.Next(1000, 9999)}";
            if (!await _batchRepository.ExistsByBatchCodeAsync(candidate, cancellationToken: cancellationToken))
            {
                return candidate;
            }
        }

        throw new InvalidOperationException("Impossibile generare un codice lotto univoco.");
    }

    private static string? Normalize(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
