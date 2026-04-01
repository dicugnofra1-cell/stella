using System;
using System.Collections.Generic;

namespace Mandorle.Domain.Entities;

public partial class Product
{
    public int Id { get; set; }

    public string Sku { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string UnitOfMeasure { get; set; } = null!;

    public string? Category { get; set; }

    public bool ChannelB2BEnabled { get; set; }

    public bool ChannelB2CEnabled { get; set; }

    public bool Active { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Batch> Batches { get; set; } = new List<Batch>();

    public virtual ICollection<InventoryMovement> InventoryMovements { get; set; } = new List<InventoryMovement>();

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual ICollection<StockReservation> StockReservations { get; set; } = new List<StockReservation>();
}
