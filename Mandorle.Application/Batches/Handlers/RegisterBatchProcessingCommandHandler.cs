using Mandorle.Application.Batches.Commands;
using Mandorle.Application.Batches.Mapping;
using Mandorle.Application.Batches.Models;
using Mandorle.Domain.Entities;
using Mandorle.Domain.Enums;
using Mandorle.Domain.Interfaces;
using MediatR;

namespace Mandorle.Application.Batches.Handlers;

public class RegisterBatchProcessingCommandHandler : IRequestHandler<RegisterBatchProcessingCommand, BatchProcessingResultDto>
{
    private readonly IBatchRepository _batchRepository;
    private readonly IBatchLinkRepository _batchLinkRepository;
    private readonly IInventoryMovementRepository _inventoryMovementRepository;

    public RegisterBatchProcessingCommandHandler(
        IBatchRepository batchRepository,
        IBatchLinkRepository batchLinkRepository,
        IInventoryMovementRepository inventoryMovementRepository)
    {
        _batchRepository = batchRepository;
        _batchLinkRepository = batchLinkRepository;
        _inventoryMovementRepository = inventoryMovementRepository;
    }

    public async Task<BatchProcessingResultDto> Handle(RegisterBatchProcessingCommand request, CancellationToken cancellationToken)
    {
        ValidateRequest(request);

        var sources = new List<(Batch Batch, decimal Quantity)>();
        foreach (var source in request.Sources)
        {
            var batch = await _batchRepository.GetByIdAsync(source.BatchId, cancellationToken)
                ?? throw new InvalidOperationException("Uno dei lotti selezionati non esiste.");

            if (batch.BioFlag)
            {
                throw new InvalidOperationException("I lotti BIO non possono essere uniti in questa versione del sistema.");
            }

            if (!OperationalEnumMappings.TryParseBatchStatus(batch.Status, out var batchStatus) || !batchStatus.IsEligibleForSale())
            {
                throw new InvalidOperationException("Uno dei lotti selezionati non e disponibile per la lavorazione.");
            }

            sources.Add((batch, source.Quantity));
        }

        EnsureSourcesAreCompatible(sources);
        await EnsureAvailabilityAsync(sources, cancellationToken);

        var batchCode = await GenerateDerivedBatchCodeAsync("LAV", cancellationToken);
        var firstBatch = sources[0].Batch;
        var totalSourceQuantity = sources.Sum(item => item.Quantity);

        if (request.ResultQuantity > totalSourceQuantity)
        {
            throw new InvalidOperationException("La quantita risultante non puo superare la quantita totale prelevata dai lotti origine.");
        }

        var derivedBatch = new Batch
        {
            BatchCode = batchCode,
            ProductId = firstBatch.ProductId,
            BatchType = Normalize(request.ResultBatchType)!,
            Status = BatchStatus.Disponibile.ToDbValue(),
            BioFlag = false,
            Variety = ResolveVariety(sources.Select(item => item.Batch).ToList()),
            InitialQuantity = request.ResultQuantity,
            UnitOfMeasure = Normalize(request.UnitOfMeasure)!,
            SupplierId = ResolveSupplierId(sources.Select(item => item.Batch).ToList()),
            SupplierDocumentId = null,
            CertificationId = null,
            ProductionDate = DateOnly.FromDateTime(DateTime.UtcNow),
            ExpirationDate = null,
            Notes = Normalize(request.Notes),
            CreatedAt = DateTime.UtcNow
        };

        await _batchRepository.AddAsync(derivedBatch, cancellationToken);

        var createdLinks = new List<BatchLink>();
        foreach (var source in sources)
        {
            var unloadMovement = new InventoryMovement
            {
                ProductId = source.Batch.ProductId,
                BatchId = source.Batch.Id,
                MovementType = InventoryMovementType.Unload.ToDbValue(),
                Quantity = source.Quantity,
                MovementDate = DateTime.UtcNow,
                Reason = "LAVORAZIONE_USCITA",
                ReferenceType = MovementReferenceType.Processing.ToDbValue(),
                ReferenceId = batchCode,
                UserId = request.UserId.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            await _inventoryMovementRepository.AddAsync(unloadMovement, cancellationToken);

            var link = new BatchLink
            {
                ParentBatchId = source.Batch.Id,
                ChildBatch = derivedBatch,
                QuantityUsed = source.Quantity,
                UnitOfMeasure = request.UnitOfMeasure.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            await _batchLinkRepository.AddAsync(link, cancellationToken);
            createdLinks.Add(link);
        }

        var loadMovement = new InventoryMovement
        {
            ProductId = firstBatch.ProductId,
            Batch = derivedBatch,
            MovementType = InventoryMovementType.Load.ToDbValue(),
            Quantity = request.ResultQuantity,
            MovementDate = DateTime.UtcNow,
            Reason = $"LAVORAZIONE_{Normalize(request.ProcessingType)}",
            ReferenceType = MovementReferenceType.Processing.ToDbValue(),
            ReferenceId = batchCode,
            UserId = request.UserId.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        await _inventoryMovementRepository.AddAsync(loadMovement, cancellationToken);

        await _batchRepository.SaveChangesAsync(cancellationToken);
        await _inventoryMovementRepository.SaveChangesAsync(cancellationToken);
        await _batchLinkRepository.SaveChangesAsync(cancellationToken);

        return new BatchProcessingResultDto(
            derivedBatch.ToDto(),
            createdLinks.Select(link => link.ToDto()).ToList(),
            request.ResultQuantity,
            request.UnitOfMeasure.Trim());
    }

    private static void ValidateRequest(RegisterBatchProcessingCommand request)
    {
        if (request.Sources is null || request.Sources.Count < 2)
        {
            throw new InvalidOperationException("Per registrare una lavorazione servono almeno due lotti origine.");
        }

        if (request.Sources.Any(source => source.Quantity <= 0))
        {
            throw new InvalidOperationException("Ogni lotto origine deve avere una quantita maggiore di zero.");
        }

        if (request.ResultQuantity <= 0)
        {
            throw new InvalidOperationException("La quantita risultante deve essere maggiore di zero.");
        }

        if (string.IsNullOrWhiteSpace(request.UnitOfMeasure))
        {
            throw new InvalidOperationException("L'unita di misura e obbligatoria.");
        }

        if (string.IsNullOrWhiteSpace(request.ResultBatchType))
        {
            throw new InvalidOperationException("La tipologia del lotto risultante e obbligatoria.");
        }

        if (string.IsNullOrWhiteSpace(request.ProcessingType))
        {
            throw new InvalidOperationException("Il tipo di lavorazione e obbligatorio.");
        }

        if (string.IsNullOrWhiteSpace(request.UserId))
        {
            throw new InvalidOperationException("L'utente che registra la lavorazione e obbligatorio.");
        }
    }

    private static void EnsureSourcesAreCompatible(IReadOnlyList<(Batch Batch, decimal Quantity)> sources)
    {
        var productId = sources[0].Batch.ProductId;
        var batchType = Normalize(sources[0].Batch.BatchType);
        var unitOfMeasure = Normalize(sources[0].Batch.UnitOfMeasure);

        if (sources.Any(source => source.Batch.ProductId != productId))
        {
            throw new InvalidOperationException("I lotti selezionati devono appartenere allo stesso prodotto.");
        }

        if (sources.Any(source => Normalize(source.Batch.BatchType) != batchType))
        {
            throw new InvalidOperationException("I lotti selezionati devono avere la stessa tipologia.");
        }

        var variety = Normalize(sources[0].Batch.Variety);
        if (sources.Any(source => Normalize(source.Batch.Variety) != variety))
        {
            throw new InvalidOperationException("I lotti selezionati devono avere la stessa varieta. Non e possibile unire qualita diverse di mandorle.");
        }

        if (sources.Any(source => Normalize(source.Batch.UnitOfMeasure) != unitOfMeasure))
        {
            throw new InvalidOperationException("I lotti selezionati devono avere la stessa unita di misura.");
        }

        var distinctBatchIds = sources.Select(source => source.Batch.Id).Distinct().Count();
        if (distinctBatchIds != sources.Count)
        {
            throw new InvalidOperationException("Non puoi selezionare lo stesso lotto due volte nella stessa lavorazione.");
        }
    }

    private async Task EnsureAvailabilityAsync(IReadOnlyList<(Batch Batch, decimal Quantity)> sources, CancellationToken cancellationToken)
    {
        foreach (var source in sources)
        {
            var physicalStock = await _inventoryMovementRepository.GetBalanceByBatchAsync(source.Batch.Id, cancellationToken);
            if (physicalStock < source.Quantity)
            {
                throw new InvalidOperationException($"Il lotto {source.Batch.BatchCode} non ha disponibilita sufficiente per la lavorazione.");
            }
        }
    }

    private async Task<string> GenerateDerivedBatchCodeAsync(string prefix, CancellationToken cancellationToken)
    {
        for (var attempt = 0; attempt < 20; attempt++)
        {
            var candidate = $"{prefix}-{DateTime.UtcNow:yyMM}-{Random.Shared.Next(1, 9999):0000}";
            if (!await _batchRepository.ExistsByBatchCodeAsync(candidate, cancellationToken: cancellationToken))
            {
                return candidate;
            }
        }

        throw new InvalidOperationException("Impossibile generare un codice lotto derivato univoco.");
    }

    private static string? ResolveVariety(IReadOnlyList<Batch> batches)
    {
        var values = batches
            .Select(batch => Normalize(batch.Variety))
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        return values.Count switch
        {
            0 => null,
            1 => values[0],
            _ => throw new InvalidOperationException("I lotti selezionati devono avere la stessa varieta.")
        };
    }

    private static int? ResolveSupplierId(IReadOnlyList<Batch> batches)
    {
        var values = batches
            .Select(batch => batch.SupplierId)
            .Distinct()
            .ToList();

        return values.Count == 1 ? values[0] : null;
    }

    private static string? Normalize(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim().ToUpperInvariant();
    }
}
