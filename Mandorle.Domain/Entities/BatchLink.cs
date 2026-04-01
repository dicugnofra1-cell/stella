using System;
using System.Collections.Generic;

namespace Mandorle.Domain.Entities;

public partial class BatchLink
{
    public int Id { get; set; }

    public int ParentBatchId { get; set; }

    public int ChildBatchId { get; set; }

    public decimal QuantityUsed { get; set; }

    public string UnitOfMeasure { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual Batch ChildBatch { get; set; } = null!;

    public virtual Batch ParentBatch { get; set; } = null!;
}
