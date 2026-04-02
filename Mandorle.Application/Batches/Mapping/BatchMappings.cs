using Mandorle.Application.Batches.Models;
using Mandorle.Domain.Entities;

namespace Mandorle.Application.Batches.Mapping;

internal static class BatchMappings
{
    public static BatchDto ToDto(this Batch batch)
    {
        return new BatchDto(
            batch.Id,
            batch.BatchCode,
            batch.ProductId,
            batch.BatchType,
            batch.Status,
            batch.BioFlag,
            batch.SupplierId,
            batch.ProductionDate,
            batch.ExpirationDate,
            batch.Notes);
    }
}
