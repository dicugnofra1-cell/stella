using System;
using System.Collections.Generic;

namespace Mandorle.Domain.Entities;

public partial class InventoryMovement
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public int BatchId { get; set; }

    public decimal Quantity { get; set; }

    public string MovementType { get; set; } = null!;

    public DateTime? MovementDate { get; set; }

    public string? Reference { get; set; }

    public virtual Batch Batch { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
