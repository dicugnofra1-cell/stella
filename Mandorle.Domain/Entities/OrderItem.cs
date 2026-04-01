using System;
using System.Collections.Generic;

namespace Mandorle.Domain.Entities;

public partial class OrderItem
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public int ProductId { get; set; }

    public decimal Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal? TaxAmount { get; set; }

    public int? ReservedBatchId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;

    public virtual Batch? ReservedBatch { get; set; }

    public virtual ICollection<StockReservation> StockReservations { get; set; } = new List<StockReservation>();
}
