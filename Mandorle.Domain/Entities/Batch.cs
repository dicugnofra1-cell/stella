using System;
using System.Collections.Generic;

namespace Mandorle.Domain.Entities;

public partial class Batch
{
    public int Id { get; set; }

    public string BatchCode { get; set; } = null!;

    public int ProductId { get; set; }

    public string BatchType { get; set; } = null!;

    public string Status { get; set; } = null!;

    public bool BioFlag { get; set; }

    public int? SupplierId { get; set; }

    public DateOnly? ProductionDate { get; set; }

    public DateOnly? ExpirationDate { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<BatchLink> BatchLinkChildBatches { get; set; } = new List<BatchLink>();

    public virtual ICollection<BatchLink> BatchLinkParentBatches { get; set; } = new List<BatchLink>();

    public virtual ICollection<InventoryMovement> InventoryMovements { get; set; } = new List<InventoryMovement>();

    public virtual ICollection<NonConformity> NonConformities { get; set; } = new List<NonConformity>();

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual Product Product { get; set; } = null!;

    public virtual PublicTraceView? PublicTraceView { get; set; }

    public virtual ICollection<QualityCheck> QualityChecks { get; set; } = new List<QualityCheck>();

    public virtual ICollection<StockReservation> StockReservations { get; set; } = new List<StockReservation>();

    public virtual Supplier? Supplier { get; set; }
}
