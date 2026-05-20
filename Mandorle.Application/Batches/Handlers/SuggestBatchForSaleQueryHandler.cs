using Mandorle.Application.Batches.Models;
using Mandorle.Application.Batches.Queries;
using Mandorle.Domain.Enums;
using Mandorle.Domain.Interfaces;
using MediatR;

namespace Mandorle.Application.Batches.Handlers;

public class SuggestBatchForSaleQueryHandler : IRequestHandler<SuggestBatchForSaleQuery, BatchSaleSuggestionDto?>
{
    private readonly IBatchRepository _batchRepository;
    private readonly IInventoryMovementRepository _inventoryMovementRepository;
    private readonly IStockReservationRepository _stockReservationRepository;

    public SuggestBatchForSaleQueryHandler(
        IBatchRepository batchRepository,
        IInventoryMovementRepository inventoryMovementRepository,
        IStockReservationRepository stockReservationRepository)
    {
        _batchRepository = batchRepository;
        _inventoryMovementRepository = inventoryMovementRepository;
        _stockReservationRepository = stockReservationRepository;
    }

    public async Task<BatchSaleSuggestionDto?> Handle(SuggestBatchForSaleQuery request, CancellationToken cancellationToken)
    {
        if (request.Quantity <= 0)
        {
            throw new InvalidOperationException("La quantità richiesta deve essere maggiore di zero.");
        }

        var candidates = await _batchRepository.GetSaleCandidatesAsync(
            request.ProductId,
            request.BatchType,
            request.BioFlag,
            request.Variety,
            cancellationToken);

        foreach (var batch in candidates)
        {
            if (!OperationalEnumMappings.TryParseBatchStatus(batch.Status, out var batchStatus) || !batchStatus.IsEligibleForSale())
            {
                continue;
            }

            var physicalStock = await _inventoryMovementRepository.GetBalanceByBatchAsync(batch.Id, cancellationToken);
            var reservedStock = await _stockReservationRepository.GetReservedQuantityByBatchAsync(batch.Id, cancellationToken);
            var availableStock = physicalStock - reservedStock;

            if (availableStock < request.Quantity)
            {
                continue;
            }

            return new BatchSaleSuggestionDto(
                batch.Id,
                batch.BatchCode,
                batch.ProductId,
                batch.BatchType,
                batch.BioFlag,
                physicalStock,
                reservedStock,
                availableStock);
        }

        return null;
    }
}
