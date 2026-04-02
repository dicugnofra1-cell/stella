using Mandorle.Application.Quality.Models;
using Mandorle.Domain.Entities;

namespace Mandorle.Application.Quality.Mapping;

internal static class QualityMappings
{
    public static QualityCheckDto ToDto(this QualityCheck qualityCheck)
    {
        return new QualityCheckDto(
            qualityCheck.Id,
            qualityCheck.BatchId,
            qualityCheck.ChecklistVersion,
            qualityCheck.Result,
            qualityCheck.Notes,
            qualityCheck.CheckedAt,
            qualityCheck.CheckedBy);
    }
}
