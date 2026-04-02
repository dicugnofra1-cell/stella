using Mandorle.Application.Traceability.Models;
using Mandorle.Domain.Entities;

namespace Mandorle.Application.Traceability.Mapping;

internal static class PublicTraceViewMappings
{
    public static PublicTraceViewDto ToDto(this PublicTraceView traceView)
    {
        return new PublicTraceViewDto(
            traceView.BatchId,
            traceView.BatchCode,
            traceView.ProductName,
            traceView.BioFlag,
            traceView.OriginInfo,
            traceView.MainDates,
            traceView.LastUpdatedAt);
    }
}
