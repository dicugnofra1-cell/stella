using Mandorle.Application.Traceability.Models;
using Mandorle.Application.Traceability.Queries;
using Mandorle.Domain.Entities;
using Mandorle.Domain.Enums;
using Mandorle.Domain.Interfaces;
using MediatR;

namespace Mandorle.Application.Traceability.Handlers;

public class GetPublicTraceViewByBatchIdQueryHandler : IRequestHandler<GetPublicTraceViewByBatchIdQuery, PublicTraceViewDto?>
{
    private readonly IPublicTraceViewRepository _publicTraceViewRepository;
    private readonly IBatchRepository _batchRepository;
    private readonly IProductRepository _productRepository;
    private readonly ISupplierRepository _supplierRepository;
    private readonly ICertificationRepository _certificationRepository;
    private readonly IQualityCheckRepository _qualityCheckRepository;
    private readonly IInventoryMovementRepository _inventoryMovementRepository;
    private readonly IBatchLinkRepository _batchLinkRepository;

    public GetPublicTraceViewByBatchIdQueryHandler(
        IPublicTraceViewRepository publicTraceViewRepository,
        IBatchRepository batchRepository,
        IProductRepository productRepository,
        ISupplierRepository supplierRepository,
        ICertificationRepository certificationRepository,
        IQualityCheckRepository qualityCheckRepository,
        IInventoryMovementRepository inventoryMovementRepository,
        IBatchLinkRepository batchLinkRepository)
    {
        _publicTraceViewRepository = publicTraceViewRepository;
        _batchRepository = batchRepository;
        _productRepository = productRepository;
        _supplierRepository = supplierRepository;
        _certificationRepository = certificationRepository;
        _qualityCheckRepository = qualityCheckRepository;
        _inventoryMovementRepository = inventoryMovementRepository;
        _batchLinkRepository = batchLinkRepository;
    }

    public async Task<PublicTraceViewDto?> Handle(GetPublicTraceViewByBatchIdQuery request, CancellationToken cancellationToken)
    {
        var traceView = await _publicTraceViewRepository.GetByBatchIdAsync(request.BatchId, cancellationToken);
        if (traceView is null)
        {
            return null;
        }

        return await BuildTraceabilityDtoAsync(traceView, cancellationToken);
    }

    private async Task<PublicTraceViewDto> BuildTraceabilityDtoAsync(PublicTraceView traceView, CancellationToken cancellationToken)
    {
        var batch = await _batchRepository.GetByIdAsync(traceView.BatchId, cancellationToken)
            ?? throw new InvalidOperationException("Il lotto associato alla tracciabilita non esiste.");
        var product = await _productRepository.GetByIdAsync(batch.ProductId, cancellationToken);
        var supplier = batch.SupplierId.HasValue
            ? await _supplierRepository.GetByIdAsync(batch.SupplierId.Value, cancellationToken)
            : null;
        var certifications = batch.SupplierId.HasValue
            ? await _certificationRepository.GetBySupplierIdAsync(batch.SupplierId.Value, status: CertificationStatus.Active.ToDbValue(), cancellationToken: cancellationToken)
            : Array.Empty<Certification>();
        var qualityChecks = await _qualityCheckRepository.GetByBatchIdAsync(batch.Id, cancellationToken);
        var movements = await _inventoryMovementRepository.SearchAsync(null, batch.Id, null, null, null, cancellationToken);
        var parentLinks = await _batchLinkRepository.GetByChildBatchIdAsync(batch.Id, cancellationToken);

        var latestQualityCheck = qualityChecks
            .OrderByDescending(check => check.CheckedAt)
            .FirstOrDefault();

        var relevantActivities = new List<string>();

        if (parentLinks.Count > 0)
        {
            relevantActivities.Add($"Lavorazione registrata da lotto origine: {parentLinks.Count} collegamento/i.");
        }

        foreach (var movement in movements.Take(3))
        {
            relevantActivities.Add(FormatMovement(movement));
        }

        return new PublicTraceViewDto(
            traceView.BatchId,
            traceView.BatchCode,
            product?.Name ?? traceView.ProductName,
            traceView.BioFlag,
            traceView.OriginInfo,
            traceView.MainDates,
            traceView.LastUpdatedAt,
            "Stella",
            supplier?.Name,
            ExtractVariety(batch.Notes),
            BuildCertificationInfo(traceView.BioFlag, certifications),
            latestQualityCheck is null ? null : $"Controllo qualita: {latestQualityCheck.Result} - {latestQualityCheck.CheckedAt:dd/MM/yyyy}",
            relevantActivities);
    }

    private static string? ExtractVariety(string? notes)
    {
        if (string.IsNullOrWhiteSpace(notes))
        {
            return null;
        }

        const string prefix = "Varieta: ";
        var startIndex = notes.IndexOf(prefix, StringComparison.OrdinalIgnoreCase);
        if (startIndex < 0)
        {
            return null;
        }

        startIndex += prefix.Length;
        var endIndex = notes.IndexOf(" | ", startIndex, StringComparison.Ordinal);
        return endIndex >= 0
            ? notes[startIndex..endIndex].Trim()
            : notes[startIndex..].Trim();
    }

    private static string? BuildCertificationInfo(bool bioFlag, IReadOnlyList<Certification> certifications)
    {
        if (!bioFlag)
        {
            return "Non BIO";
        }

        var certification = certifications
            .OrderByDescending(item => item.ExpiryDate)
            .FirstOrDefault();

        return certification is null
            ? "BIO senza certificazione pubblica disponibile"
            : $"BIO certificato - {certification.Type} ({certification.Authority})";
    }

    private static string FormatMovement(InventoryMovement movement)
    {
        var direction = movement.MovementType.Equals(InventoryMovementType.Unload.ToDbValue(), StringComparison.OrdinalIgnoreCase)
            ? "-"
            : "+";

        return $"{movement.MovementDate:dd/MM/yyyy} - {movement.Reason ?? movement.MovementType}: {direction}{movement.Quantity}";
    }
}
