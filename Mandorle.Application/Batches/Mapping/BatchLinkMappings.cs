using Mandorle.Application.Batches.Models;
using Mandorle.Domain.Entities;

namespace Mandorle.Application.Batches.Mapping;

internal static class BatchLinkMappings
{
    public static BatchLinkDto ToDto(this BatchLink batchLink)
    {
        return new BatchLinkDto(
            batchLink.Id,
            batchLink.ParentBatchId,
            batchLink.ChildBatchId,
            batchLink.QuantityUsed,
            batchLink.UnitOfMeasure);
    }
}
