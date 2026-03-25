using System;
using System.Collections.Generic;

namespace Mandorle.Domain.Entities;

public partial class Batch
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public string BatchCode { get; set; } = null!;

    public string Type { get; set; } = null!;

    public bool IsBio { get; set; }

    public int? ParentBatchId { get; set; }

    public DateTime? ProductionDate { get; set; }

    public DateTime? ExpirationDate { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<InventoryMovement> InventoryMovements { get; set; } = new List<InventoryMovement>();

    public virtual ICollection<Batch> InverseParentBatch { get; set; } = new List<Batch>();

    public virtual Batch? ParentBatch { get; set; }

    public virtual Product Product { get; set; } = null!;
}
