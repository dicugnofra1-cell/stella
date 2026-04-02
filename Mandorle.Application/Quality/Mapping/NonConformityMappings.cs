using Mandorle.Application.Quality.Models;
using Mandorle.Domain.Entities;

namespace Mandorle.Application.Quality.Mapping;

internal static class NonConformityMappings
{
    public static NonConformityDto ToDto(this NonConformity nonConformity)
    {
        return new NonConformityDto(
            nonConformity.Id,
            nonConformity.BatchId,
            nonConformity.Severity,
            nonConformity.Status,
            nonConformity.Description,
            nonConformity.CorrectiveAction,
            nonConformity.OpenedBy,
            nonConformity.OpenedAt,
            nonConformity.ClosedBy,
            nonConformity.ClosedAt);
    }
}
