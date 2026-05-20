using Mandorle.Application.Batches.Mapping;
using Mandorle.Application.GoodsReceipts.Commands;
using Mandorle.Application.GoodsReceipts.Models;
using Mandorle.Application.Inventory.Mapping;
using Mandorle.Domain.Entities;
using Mandorle.Domain.Enums;
using Mandorle.Domain.Interfaces;
using MediatR;

namespace Mandorle.Application.GoodsReceipts.Handlers;

public class UpdateGoodsReceiptCommandHandler : IRequestHandler<UpdateGoodsReceiptCommand, GoodsReceiptDto>
{
    private readonly IBatchRepository _batchRepository;
    private readonly IProductRepository _productRepository;
    private readonly ISupplierRepository _supplierRepository;
    private readonly ISupplierDocumentRepository _supplierDocumentRepository;
    private readonly ICertificationRepository _certificationRepository;
    private readonly IInventoryMovementRepository _inventoryMovementRepository;
    private readonly IStockReservationRepository _stockReservationRepository;
    private readonly IBatchLinkRepository _batchLinkRepository;

    public UpdateGoodsReceiptCommandHandler(
        IBatchRepository batchRepository,
        IProductRepository productRepository,
        ISupplierRepository supplierRepository,
        ISupplierDocumentRepository supplierDocumentRepository,
        ICertificationRepository certificationRepository,
        IInventoryMovementRepository inventoryMovementRepository,
        IStockReservationRepository stockReservationRepository,
        IBatchLinkRepository batchLinkRepository)
    {
        _batchRepository = batchRepository;
        _productRepository = productRepository;
        _supplierRepository = supplierRepository;
        _supplierDocumentRepository = supplierDocumentRepository;
        _certificationRepository = certificationRepository;
        _inventoryMovementRepository = inventoryMovementRepository;
        _stockReservationRepository = stockReservationRepository;
        _batchLinkRepository = batchLinkRepository;
    }

    public async Task<GoodsReceiptDto> Handle(UpdateGoodsReceiptCommand request, CancellationToken cancellationToken)
    {
        if (request.Quantity <= 0)
        {
            throw new InvalidOperationException("La quantita deve essere maggiore di zero.");
        }

        if (request.PurchaseUnitPrice <= 0)
        {
            throw new InvalidOperationException("Il prezzo di acquisto deve essere maggiore di zero.");
        }

        if (string.IsNullOrWhiteSpace(request.UnitOfMeasure))
        {
            throw new InvalidOperationException("L'unita di misura e obbligatoria.");
        }

        if (string.IsNullOrWhiteSpace(request.UserId))
        {
            throw new InvalidOperationException("L'utente che registra la modifica e obbligatorio.");
        }

        var batch = await _batchRepository.GetByIdAsync(request.BatchId, cancellationToken)
            ?? throw new InvalidOperationException("L'ingresso merce selezionato non esiste.");

        await EnsureBatchIsStillEditableAsync(batch, cancellationToken);

        var loadMovement = await ResolveLoadMovementAsync(batch.Id, cancellationToken);
        var product = await ResolveProductAsync(request, cancellationToken);
        var supplier = await ResolveSupplierAsync(request, cancellationToken);
        var supplierDocument = await ValidateSupplierDocumentAsync(supplier.Id, request.SupplierDocumentId, cancellationToken);
        var certification = await ValidateCertificationAsync(request, supplier.Id, cancellationToken);

        var shouldRegenerateBatchCode = ShouldRegenerateBatchCode(batch, request, product.Id, supplier.Id);
        if (shouldRegenerateBatchCode)
        {
            batch.BatchCode = await GenerateBatchCodeAsync(cancellationToken);
        }

        batch.ProductId = product.Id;
        batch.BatchType = Normalize(request.BatchType)!;
        batch.Status = BatchStatus.Ricevuto.ToDbValue();
        batch.BioFlag = request.BioFlag;
        batch.Variety = Normalize(request.Variety);
        batch.InitialQuantity = request.Quantity;
        batch.PurchaseUnitPrice = request.PurchaseUnitPrice;
        batch.UnitOfMeasure = Normalize(request.UnitOfMeasure)!;
        batch.SupplierId = supplier.Id;
        batch.SupplierDocumentId = supplierDocument?.Id;
        batch.CertificationId = certification?.Id;
        batch.ProductionDate = request.ProductionDate;
        batch.ExpirationDate = request.ExpirationDate;
        batch.Notes = Normalize(request.Notes);
        batch.UpdatedAt = DateTime.UtcNow;

        loadMovement.ProductId = product.Id;
        loadMovement.Quantity = request.Quantity;
        loadMovement.MovementDate = request.ReceivedAt ?? loadMovement.MovementDate;
        loadMovement.ReferenceId = batch.BatchCode;
        loadMovement.UserId = Normalize(request.UserId)!;

        _batchRepository.Update(batch);
        await _inventoryMovementRepository.SaveChangesAsync(cancellationToken);
        await _batchRepository.SaveChangesAsync(cancellationToken);

        return new GoodsReceiptDto(
            batch.ToDto(),
            loadMovement.ToDto(),
            request.Quantity,
            request.PurchaseUnitPrice,
            Normalize(request.UnitOfMeasure)!,
            supplierDocument?.Id,
            certification?.Id);
    }

    private async Task EnsureBatchIsStillEditableAsync(Batch batch, CancellationToken cancellationToken)
    {
        var reservations = await _stockReservationRepository.SearchAsync(
            orderId: null,
            orderItemId: null,
            productId: null,
            batchId: batch.Id,
            status: null,
            reservationType: null,
            cancellationToken: cancellationToken);

        if (reservations.Count > 0)
        {
            throw new InvalidOperationException("Questo ingresso non puo piu essere modificato perche il lotto ha gia riserve collegate.");
        }

        var parentLinks = await _batchLinkRepository.GetByParentBatchIdAsync(batch.Id, cancellationToken);
        var childLinks = await _batchLinkRepository.GetByChildBatchIdAsync(batch.Id, cancellationToken);
        if (parentLinks.Count > 0 || childLinks.Count > 0)
        {
            throw new InvalidOperationException("Questo ingresso non puo piu essere modificato perche il lotto e gia coinvolto in una lavorazione.");
        }

        var movements = await _inventoryMovementRepository.SearchAsync(
            productId: null,
            batchId: batch.Id,
            movementType: null,
            fromDate: null,
            toDate: null,
            cancellationToken: cancellationToken);

        if (movements.Any(movement => !string.Equals(movement.MovementType, InventoryMovementType.Load.ToDbValue(), StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException("Questo ingresso non puo piu essere modificato perche il lotto ha gia movimenti operativi successivi.");
        }

        if (movements.Count(movement => string.Equals(movement.MovementType, InventoryMovementType.Load.ToDbValue(), StringComparison.OrdinalIgnoreCase)) != 1)
        {
            throw new InvalidOperationException("Non sono riuscita a trovare un solo movimento di ingresso da aggiornare.");
        }
    }

    private async Task<InventoryMovement> ResolveLoadMovementAsync(int batchId, CancellationToken cancellationToken)
    {
        var movement = (await _inventoryMovementRepository.SearchAsync(
                productId: null,
                batchId: batchId,
                movementType: InventoryMovementType.Load.ToDbValue(),
                fromDate: null,
                toDate: null,
                cancellationToken: cancellationToken))
            .OrderBy(item => item.MovementDate)
            .ThenBy(item => item.Id)
            .FirstOrDefault();

        if (movement is null)
        {
            throw new InvalidOperationException("Non sono riuscita a trovare il movimento di ingresso da aggiornare.");
        }

        return await _inventoryMovementRepository.GetByIdAsync(movement.Id, cancellationToken)
            ?? throw new InvalidOperationException("Non sono riuscita a caricare il movimento di ingresso da aggiornare.");
    }

    private bool ShouldRegenerateBatchCode(Batch batch, UpdateGoodsReceiptCommand request, int resolvedProductId, int resolvedSupplierId)
    {
        return batch.ProductId != resolvedProductId
            || batch.SupplierId != resolvedSupplierId
            || !string.Equals(Normalize(batch.BatchType), Normalize(request.BatchType), StringComparison.Ordinal)
            || batch.BioFlag != request.BioFlag
            || !string.Equals(Normalize(batch.Variety), Normalize(request.Variety), StringComparison.Ordinal);
    }

    private async Task<Product> ResolveProductAsync(UpdateGoodsReceiptCommand request, CancellationToken cancellationToken)
    {
        var hasProductId = request.ProductId.HasValue;
        var hasNewProduct = request.NewProduct is not null;

        if (hasProductId == hasNewProduct)
        {
            throw new InvalidOperationException("Indica un prodotto esistente oppure un nuovo prodotto da creare.");
        }

        if (hasProductId)
        {
            var existingProduct = await _productRepository.GetByIdAsync(request.ProductId!.Value, cancellationToken)
                ?? throw new InvalidOperationException("Il prodotto selezionato non esiste.");

            EnsureBatchTypeMatchesProduct(existingProduct, request.BatchType);
            return existingProduct;
        }

        var newProduct = request.NewProduct!;
        if (string.IsNullOrWhiteSpace(newProduct.Name))
        {
            throw new InvalidOperationException("Il nome del nuovo prodotto e obbligatorio.");
        }

        if (string.IsNullOrWhiteSpace(newProduct.UnitOfMeasure))
        {
            throw new InvalidOperationException("L'unita di misura del nuovo prodotto e obbligatoria.");
        }

        if (string.IsNullOrWhiteSpace(newProduct.DefaultBatchType))
        {
            throw new InvalidOperationException("La tipologia predefinita del prodotto e obbligatoria.");
        }

        var sku = await GenerateProductSkuAsync(newProduct, request.BatchType, cancellationToken);
        var product = new Product
        {
            Sku = sku,
            Name = Normalize(newProduct.Name)!,
            Description = Normalize(newProduct.Description),
            UnitOfMeasure = Normalize(newProduct.UnitOfMeasure)!,
            Category = Normalize(newProduct.Category) ?? Normalize(request.BatchType),
            DefaultBatchType = Normalize(newProduct.DefaultBatchType)!,
            ChannelB2BEnabled = newProduct.ChannelB2BEnabled ?? true,
            ChannelB2CEnabled = newProduct.ChannelB2CEnabled ?? true,
            Active = newProduct.Active ?? true,
            CreatedAt = DateTime.UtcNow
        };

        EnsureBatchTypeMatchesProduct(product, request.BatchType);

        await _productRepository.AddAsync(product, cancellationToken);
        await _productRepository.SaveChangesAsync(cancellationToken);

        return product;
    }

    private async Task<Supplier> ResolveSupplierAsync(UpdateGoodsReceiptCommand request, CancellationToken cancellationToken)
    {
        var hasSupplierId = request.SupplierId.HasValue;
        var hasNewSupplier = request.NewSupplier is not null;

        if (hasSupplierId == hasNewSupplier)
        {
            throw new InvalidOperationException("Indica un fornitore esistente oppure un nuovo fornitore da creare.");
        }

        if (hasSupplierId)
        {
            return await _supplierRepository.GetByIdAsync(request.SupplierId!.Value, cancellationToken)
                ?? throw new InvalidOperationException("Il fornitore selezionato non esiste.");
        }

        if (request.SupplierDocumentId.HasValue || request.CertificationId.HasValue)
        {
            throw new InvalidOperationException("Per un nuovo fornitore, documenti e certificazioni vanno caricati dopo la creazione anagrafica.");
        }

        var newSupplier = request.NewSupplier!;
        if (string.IsNullOrWhiteSpace(newSupplier.Name))
        {
            throw new InvalidOperationException("Il nome del nuovo fornitore e obbligatorio.");
        }

        if (!string.IsNullOrWhiteSpace(newSupplier.Email) &&
            await _supplierRepository.ExistsByEmailAsync(newSupplier.Email, cancellationToken: cancellationToken))
        {
            throw new InvalidOperationException("Esiste gia un fornitore con la stessa email.");
        }

        if (!string.IsNullOrWhiteSpace(newSupplier.VatNumber) &&
            await _supplierRepository.ExistsByVatNumberAsync(newSupplier.VatNumber, cancellationToken: cancellationToken))
        {
            throw new InvalidOperationException("Esiste gia un fornitore con la stessa partita IVA.");
        }

        var supplier = new Supplier
        {
            Name = Normalize(newSupplier.Name)!,
            VatNumber = Normalize(newSupplier.VatNumber),
            Address = Normalize(newSupplier.Address),
            Email = Normalize(newSupplier.Email),
            Phone = Normalize(newSupplier.Phone),
            Status = Normalize(newSupplier.Status) ?? "ATTIVO",
            CreatedAt = DateTime.UtcNow
        };

        await _supplierRepository.AddAsync(supplier, cancellationToken);
        await _supplierRepository.SaveChangesAsync(cancellationToken);

        return supplier;
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

    private async Task<Certification?> ValidateCertificationAsync(UpdateGoodsReceiptCommand request, int supplierId, CancellationToken cancellationToken)
    {
        Certification? certification = null;

        if (request.CertificationId.HasValue)
        {
            certification = await _certificationRepository.GetByIdAsync(supplierId, request.CertificationId.Value, cancellationToken);
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

    private async Task<string> GenerateProductSkuAsync(RegisterGoodsReceiptProductInput input, string batchType, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(input.Sku))
        {
            var providedSku = input.Sku.Trim().ToUpperInvariant();
            if (await _productRepository.ExistsBySkuAsync(providedSku, cancellationToken: cancellationToken))
            {
                throw new InvalidOperationException("Esiste gia un prodotto con lo stesso SKU.");
            }

            return providedSku;
        }

        var nameToken = new string(input.Name
            .Trim()
            .ToUpperInvariant()
            .Where(char.IsLetterOrDigit)
            .Take(8)
            .ToArray());
        var batchToken = new string((batchType ?? string.Empty)
            .Trim()
            .ToUpperInvariant()
            .Where(char.IsLetterOrDigit)
            .Take(6)
            .ToArray());

        for (var attempt = 0; attempt < 10; attempt++)
        {
            var candidate = $"{batchToken}-{nameToken}-{Random.Shared.Next(1000, 9999)}";
            if (!await _productRepository.ExistsBySkuAsync(candidate, cancellationToken: cancellationToken))
            {
                return candidate;
            }
        }

        throw new InvalidOperationException("Impossibile generare uno SKU univoco per il nuovo prodotto.");
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
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim().ToUpperInvariant();
    }

    private static void EnsureBatchTypeMatchesProduct(Product product, string requestedBatchType)
    {
        if (!string.Equals(Normalize(product.DefaultBatchType), Normalize(requestedBatchType), StringComparison.Ordinal))
        {
            throw new InvalidOperationException("La tipologia del lotto deve essere coerente con la tipologia predefinita del prodotto selezionato.");
        }
    }
}
