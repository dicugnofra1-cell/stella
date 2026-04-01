using System;
using System.Collections.Generic;

namespace Mandorle.Domain.Entities;

public partial class InventoryMovement
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public int BatchId { get; set; }

    public string MovementType { get; set; } = null!;

    public decimal Quantity { get; set; }

    public DateTime MovementDate { get; set; }

    public string? Reason { get; set; }

    public string? ReferenceType { get; set; }

    public string? ReferenceId { get; set; }

    public string UserId { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual Batch Batch { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
