namespace Mandorle.Application.Traceability.Models;

public record PublicTraceViewDto(
    int BatchId,
    string BatchCode,
    string ProductName,
    bool BioFlag,
    string? OriginInfo,
    string? MainDates,
    DateTime LastUpdatedAt,
    string CompanyName,
    string? SupplierName,
    string? Variety,
    string? CertificationInfo,
    string? LatestQualityCheck,
    IReadOnlyList<string> RelevantActivities);
